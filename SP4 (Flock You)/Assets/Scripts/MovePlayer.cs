using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MovePlayer : MonoBehaviour
{
    public enum PlayerState
    {
        IDLE,
        DROPPING,
        JUMPING,
        FLYING,
        FLYING_HEIGHTCHANGE,// when bird is flying up / down
        FLYING_DIVEBOMB,
        FLYING_HOVERING,
        FLYING_BARREL_LEFT,
        FLYING_BARREL_RIGHT,
    }

    public enum doubletapchecktype
    {
        DOUBLE_SPACE,
        DOUBLE_NONE,
    }


    [Header("Player Settings")]
    [Tooltip("how fast can the player rotate")]
    public float turnSpeed = 2f;[Tooltip("Movement speed multiplier that affects the player's movementspeed")]
    public float movemul = 10f;[Tooltip("how high the player can jump")]
    public float jumpforce = 10f;
    public float flymaxIncrease = 2f;
    public float flymaxDecrease = 2f;[Tooltip("Determines how fast the bird rolls")]
    public float flyverticalforce = 100f;[Tooltip("Determines how fast the bird pitches")]
    public float flyhorizontalforce = 100f;[Tooltip("Determines how close the player can be before bird prepares to land")]
    public float landingheight = 4f;[Tooltip("how fast the player lands does not take into account player height")]
    public float landingtimer = 0.05f;
    public float birdterminalvelocity = -30f;
    public float AirResistance = 10f;
    public PlayerState playerState = PlayerState.IDLE;
    private float landingelapsed = 0.0f;
    private float divebombminpitch = 90.0f;
    private float fallspeed = 0f;

    private PhotonView photonView;

    [Header("Player Camera")]
    public Cinemachine.CinemachineFreeLook camera;
    public GameObject Lookat;

    Rigidbody m_Rigidbody;
    CustomGravity _customGravity;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    float horizontalmovement;
    float verticalmovement;
    float groundrag = 6f;
    float airDrag = 2f;
    float DefaultMoveMul;
    float playerheight;

    bool inputed = false;
    [SerializeField] bool isgrounded = false;
    bool islanding = false;

    // barrel roll
    public float BarrelRollSpeed = 5f;
    float BarrelRollAngle = 0;
    float rolloffsetX =0;
    float DashSpeed = 20f;

    // double jump
    double firsttaptime = 0.0;
    bool doubletapcheck = false;
    double doubletaptimeframe = 0.5f;
    float elapsedtaptime = 0f;


    [Header("Flysettings")]
    Vector3 flydrop = Vector3.zero; // for hovering
    float flaptimermax = 0.1f;
    float currenflaptimer = 0.0f;
    public Vector3 DiveBombDirection = Vector3.zero;
    [SerializeField] GameObject LeftWing = null;
    [SerializeField] GameObject RightWing = null;

    [Header("Orientation")]
    float pitch = 0.0f, yaw = 0.0f, roll = 0.0f;
    Vector3 vec3right = Vector3.zero;
    Vector3 vec3up = Vector3.zero;
    Vector3 vec3front = Vector3.zero;
    Vector3 Orientation = Vector3.zero;
    public Vector3 baserotation = Vector3.zero;

    [Header("Debug")]
    public Text directiontext;
    public Text orientationtext;

    [Header("Player Models")]
    public GameObject GroundedModel;
    public GameObject FlyingModel;

    //private doubletapchecktype doublechecktype = doubletapchecktype.DOUBLE_NONE;
    Dictionary<doubletapchecktype, double> keyfirsttaptimelist = new Dictionary<doubletapchecktype, double>();
    Dictionary<doubletapchecktype, bool> keyfirsttapstatus = new Dictionary<doubletapchecktype, bool>();
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        _customGravity = GetComponent<CustomGravity>();
        playerheight = transform.localScale.y;
        DefaultMoveMul = movemul;
        vec3up = new Vector3(0, 1, 0);
        flydrop.Set(0, GlobalSettings.gravity, 0);
        DiveBombDirection = Physics.gravity;
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
            Debug.Log("Player does not have PhotonView");
        else
        {
            if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            {
                if (!photonView.IsMine)
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }

        //apply player perk
        transform.GetComponent<PerkManager>().ApplyPlayerPerk();
    }
    private void Start()
    {
        pitch = baserotation.x;
        yaw = baserotation.y;
        roll = baserotation.z;
        m_Rigidbody.MoveRotation(Quaternion.Euler(pitch, yaw, roll));

        keyfirsttapstatus.Add(doubletapchecktype.DOUBLE_SPACE, false);

        keyfirsttaptimelist.Add(doubletapchecktype.DOUBLE_SPACE, 0);
    }

    private void Update()
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
            if (FYGameManager.Instance.isPaused)
                return;

        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;
        PlayerInput();
        controlDrag();
        Moveplayer();
        if (playerState >= PlayerState.FLYING)
        {
            //toggle flying model
            StartCoroutine(ChangeFlyingModel());
            _customGravity.enabled = false;
            Fly();
        }
        else
            _customGravity.enabled = true;

        //checkDoubletap();
        checkJump(); // checks if player jumps
        if (Input.GetKeyUp(KeyCode.C))
        {
            camera.m_LookAt = transform;
            camera.m_Follow = transform;
        }
        landingcheck();

    }

    public bool GetIsGrounded()
    {
        return isgrounded;
    }

    IEnumerator ChangeFlyingModel()
    {
        if (FlyingModel != null && GroundedModel != null)
        {
            FlyingModel.SetActive(true);
            GroundedModel.SetActive(false);
        }
        yield break;
    }

    IEnumerator ChangeGroundedModel()
    {
        if (FlyingModel != null && GroundedModel != null)
        {
            FlyingModel.SetActive(false);
            GroundedModel.SetActive(true);
        }
        yield break;
    }

    void checkJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isgrounded && playerState != PlayerState.JUMPING)
            {
                playerState = PlayerState.JUMPING;
                m_Rigidbody.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
            }
            else if (!isgrounded && playerState != PlayerState.JUMPING && playerState < PlayerState.FLYING)
            {
                playerState = PlayerState.FLYING;
                //flydrop =
                if (flydrop.y > 0)
                    flydrop *= -1;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isgrounded && playerState == PlayerState.IDLE)
        {
            playerState = PlayerState.FLYING_HEIGHTCHANGE;
        }
    }

    void PlayerInput()
    {
        horizontalmovement = Input.GetAxisRaw("Horizontal");
        verticalmovement = Input.GetAxisRaw("Vertical");
        bool hasHorizontalInput = !Mathf.Approximately(horizontalmovement, 0f);
        bool hasVerticalInput = !Mathf.Approximately(verticalmovement, 0f);
        inputed= hasHorizontalInput || hasVerticalInput;
        

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D) && playerState >= PlayerState.FLYING)
        {
            //playerState = PlayerState.FLYING_BARREL_RIGHT;
            m_Rigidbody.MovePosition(m_Rigidbody.position + (vec3right * 20f));
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.A) && playerState >= PlayerState.FLYING)
        {
            //playerState = PlayerState.FLYING_BARREL_LEFT;
            m_Rigidbody.MovePosition(m_Rigidbody.position + (-vec3right*20f));
        }
    }

    private void FixedUpdate()
    {
        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.SINGLE_PLAYER)
            if (FYGameManager.Instance.isPaused)
                return;

        if (GlobalSettings.curMode == GlobalSettings.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;
        // check player grounded
        isgrounded = Physics.Raycast(m_Rigidbody.position, Vector3.down, out RaycastHit Hit,0.1f+playerheight * 0.5f - (fallspeed * Time.deltaTime));
        if (isgrounded)
        {
            pitch = 0;
            roll = 0;
            vec3up = Vector3.up;
            toggleTrail(false);

            //toggle model to show grounded model
            StartCoroutine(ChangeGroundedModel());

            if (playerState == PlayerState.FLYING_DIVEBOMB)
                m_Rigidbody.position = Hit.point + (Vector3.up * playerheight * 0.5f);
            playerState = PlayerState.IDLE;
            fallspeed = 0;
            _customGravity.enabled = true;
        }

    }

    void Moveplayer()
    {
        Quaternion rbRotation;
        vec3front = (transform.position - camera.transform.position).normalized;
        vec3front.y = 0;
        vec3front.Normalize();

        vec3right = Vector3.Cross(vec3front, vec3up) * -1;
        
        if (inputed && playerState < PlayerState.FLYING)      //&& !isFlying)// && photonView.IsMine)
        {
            m_Rigidbody.angularVelocity = Vector3.zero;
            m_Movement = ((vec3front * verticalmovement) + (vec3right * horizontalmovement)).normalized;
            m_Rigidbody.AddForce((m_Movement * Time.deltaTime) * movemul, ForceMode.VelocityChange);
            yaw = Mathf.Rad2Deg * Mathf.Atan2(m_Movement.x, m_Movement.z);
            m_Rotation = Quaternion.AngleAxis(yaw, Vector3.up);
            rbRotation = Quaternion.Lerp(m_Rigidbody.rotation, m_Rotation, Time.deltaTime * turnSpeed);
            m_Rigidbody.MoveRotation(rbRotation);
            // Debug.Log(vec3front);
        }
    }

    void controlDrag()
    {
        if (isgrounded == true)
        {
            m_Rigidbody.drag = groundrag;
            movemul -= DefaultMoveMul * Time.deltaTime;
        }
        else if (!isgrounded)
        {
            m_Rigidbody.drag = airDrag;
            movemul -= Time.deltaTime;
        }
        if (movemul < DefaultMoveMul)
            movemul = DefaultMoveMul;
    }

    public float GetDefaultMoveMul()
    {
        return DefaultMoveMul;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("testCameraDetach"))
        {
            camera.m_LookAt = collision.transform;
            camera.Follow = collision.transform;
        }
        else
        {
            playerState = PlayerState.IDLE;
            _customGravity.enabled = true;
        }
    }

    /*public void singletapJump()
    {
        if (isgrounded)
        {
            Jump();
        }
    }*/

    public void Fly()
    {
        //checkdoubletap();
        checkDoubletap();
        if (playerState == PlayerState.FLYING_DIVEBOMB)
        {
            divebomb();
            return;
        }
        //else if (playerState == PlayerState.FLYING_BARREL_LEFT || playerState == PlayerState.FLYING_BARREL_RIGHT)
        //{
        //    BarrelRoll();
        //    return;
        //}

        if (Input.GetKey(KeyCode.Space))
        {
            float OverallForce = flyverticalforce;
            if (FYGameManager.Instance.isRaining())
                OverallForce *= 0.7f;

            //m_Movement.y += Time.deltaTime * flyverticalforce;
            if (playerState != PlayerState.FLYING_DIVEBOMB && !isgrounded)
                playerState = PlayerState.FLYING_HEIGHTCHANGE;
            if (!inputed) // striaght up
            {
                m_Rigidbody.AddForce((Vector3.up * OverallForce) * Time.deltaTime, ForceMode.VelocityChange);
                toggleTrail(false);
            }
            else // go up a relative to your orientation
            {
                Vector3 movement = Orientation;
                movement.y = 0;
                m_Rigidbody.AddForce(Quaternion.Euler(Orientation) *
                    (Vector3.up * OverallForce * Time.deltaTime), ForceMode.VelocityChange);

                if (pitch - Time.deltaTime * OverallForce < -45)
                {
                    pitch = -45;
                }
                else
                    pitch -= Time.deltaTime * OverallForce;
            }
        }
        else if (!islanding && playerState != PlayerState.FLYING_DIVEBOMB)
        {
            recenterPitch();
            playerState = PlayerState.FLYING_HOVERING;
        }


        if (Input.GetKey(KeyCode.LeftControl))
        {
            float OverallForce = flyverticalforce;
            if (FYGameManager.Instance.isRaining())
                OverallForce *= 1.5f;

            if (playerState != PlayerState.FLYING_DIVEBOMB && !isgrounded)
                playerState = PlayerState.FLYING_HEIGHTCHANGE;
            if (!inputed) // fly striaght down;
            {
                m_Rigidbody.AddForce((OverallForce * Vector3.down) * Time.deltaTime, ForceMode.VelocityChange);
                toggleTrail(false);
            }
            else // go down a relative to your orientation
            {
                movemul += (verticalmovement * 0.5f + AirResistance) * Time.deltaTime;
                recenterRoll();
                playerState = PlayerState.FLYING;
                Vector3 movement = Orientation;
                movement.y = 0;
                m_Rigidbody.AddForce(Quaternion.Euler(Orientation) * Vector3.down *
                    OverallForce * Time.deltaTime, ForceMode.VelocityChange);

                if (pitch + Time.deltaTime * OverallForce > 45)
                    pitch = 45;
                else
                    pitch += Time.deltaTime * OverallForce;
            }
        }
        else if (!islanding)
        {
            recenterPitch();
        }

        Quaternion rbRotation;
        vec3front = (m_Rigidbody.position - camera.transform.position).normalized;
        vec3front.Normalize();
        vec3front.y = 0;
        vec3right = Vector3.Cross(vec3front, vec3up) * -1;
       
        m_Rigidbody.angularVelocity = Vector3.zero;

        if (!islanding)
        {
            if (horizontalmovement != 0 && playerState != PlayerState.FLYING_HEIGHTCHANGE)
            {
                roll -= ((horizontalmovement * flyhorizontalforce) * Time.deltaTime);
                if (roll > 45)
                    roll = 45;
                else if (roll < -45)
                    roll = -45;
            }
            else
                recenterRoll();
        }
        // player has inputed then we calculate where we facing
        handleLanding(); 
        if (horizontalmovement != 0 || verticalmovement != 0)
        {
            yaw = (Mathf.Rad2Deg * Mathf.Atan2(m_Movement.x, m_Movement.z));
            m_Movement = ((vec3front * verticalmovement) + (vec3right * horizontalmovement)).normalized;
            Orientation.Set(pitch, yaw, roll);
            if (orientationtext != null)
                orientationtext.text = Orientation.ToString();
            m_Rotation = Quaternion.Euler(Orientation);
            rbRotation = Quaternion.Lerp(m_Rigidbody.rotation, m_Rotation,Time.deltaTime * turnSpeed);
            m_Rigidbody.AddForce((m_Movement * Time.deltaTime) * movemul, ForceMode.VelocityChange);
            m_Rigidbody.MoveRotation(rbRotation);
            toggleTrail(true);
        }
        else
        {
            if (playerState != PlayerState.FLYING_HEIGHTCHANGE)
            {
                simulateflap();
                toggleTrail(false);
            }
            Orientation.Set(pitch, yaw, roll);
            m_Rotation = Quaternion.Euler(Orientation);
            rbRotation = Quaternion.Lerp(m_Rigidbody.rotation, m_Rotation, Time.deltaTime * turnSpeed);
            m_Rigidbody.MoveRotation(rbRotation);
        }
    }

    void checkDoubletap()
    {
        for (doubletapchecktype i=0;i < doubletapchecktype.DOUBLE_NONE;i++)
        {
            switch (i)
            {
                case doubletapchecktype.DOUBLE_SPACE:
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        if (keyfirsttapstatus[i] == true)
                        {
                            if (Time.time - keyfirsttaptimelist[i] < doubletaptimeframe)
                            {
                                doubletapcheck = false;
                                elapsedtaptime = 0;
                                Orientation.Set(0, 0, 0);
                                toggleTrail(true);
                                if (playerState > PlayerState.FLYING && playerState != PlayerState.FLYING_DIVEBOMB)
                                {
                                    playerState = PlayerState.FLYING_DIVEBOMB;
                                    DiveBombDirection = vec3front;
                                    fallspeed = 0;
                                }
                                else if (playerState == PlayerState.FLYING_DIVEBOMB) // recover divebomb
                                {
                                    playerState = PlayerState.FLYING_HEIGHTCHANGE;
                                }
                            }
                            else // single tap jump
                            {
                                keyfirsttapstatus[i] = false;
                            }
                        }
                        else
                        {
                            keyfirsttaptimelist[i] = Time.time;
                            keyfirsttapstatus[i] = true;
                        }
                    }
                    else if (Input.GetKey(KeyCode.Space))
                    {
                        if (keyfirsttapstatus[i] == true && playerState > PlayerState.FLYING)
                        {
                            playerState = PlayerState.FLYING_DIVEBOMB;
                        }
                    }
                    else if (playerState == PlayerState.FLYING_DIVEBOMB)
                    {
                        playerState = PlayerState.FLYING;
                        movemul += -fallspeed * Time.deltaTime;
                    }
                    break;
            }
            if (Time.time - keyfirsttaptimelist[i] > doubletaptimeframe)
            {
                keyfirsttapstatus[i] = false;
            }
        }
    }


    void landingcheck()
    {
        if (playerState == PlayerState.FLYING_DIVEBOMB)
        {
            islanding = Physics.Raycast(m_Rigidbody.position, Vector3.down, 0.1f+(playerheight + landingheight) * (-2 *fallspeed));
        }
        else if (!isgrounded)
        {
            islanding = Physics.Raycast(m_Rigidbody.position, Vector3.down, 0.1f+playerheight + landingheight);
        }
        
        //Debug.Log(islanding);
    }

    void simulateflap()
    {
        if (currenflaptimer >= flaptimermax)
        {
            m_Rigidbody.AddForce(flydrop,ForceMode.Impulse);
            currenflaptimer = 0;
            flydrop *= -1;
        }
        else
            currenflaptimer += Time.deltaTime;
    }

    public void handleLanding()
    {
        if (islanding)
        {
            if (landingelapsed >= landingtimer)
            {
                landingelapsed = 0;
                pitch *= 0.25f;
                roll *= 0.25f;
            }
            else
                landingelapsed += Time.deltaTime;
        }
    }

    public void divebomb()
    {
        m_Rigidbody.angularDrag = 0;
        m_Rigidbody.angularVelocity = Vector3.zero;
        if (pitch < divebombminpitch)
        {
            pitch += flyverticalforce * Time.deltaTime;
        }
        else
            pitch = divebombminpitch;
        fallspeed += -verticalmovement * Time.deltaTime;
        Orientation.Set(pitch, yaw, roll);
        m_Movement = (verticalmovement *vec3front) + (horizontalmovement * vec3right);
        m_Movement.y = fallspeed;
        if (orientationtext != null)
            orientationtext.text = Orientation.ToString();
        m_Rotation = Quaternion.Euler(Orientation);
        m_Rotation = Quaternion.Lerp(m_Rigidbody.rotation, m_Rotation,turnSpeed * Time.deltaTime);
        m_Rigidbody.MoveRotation(m_Rotation);
        m_Rigidbody.AddForce(m_Movement* Time.deltaTime * movemul, ForceMode.Impulse);
        
        if (fallspeed > birdterminalvelocity)
            fallspeed += Physics.gravity.y * Time.deltaTime * 0.2f;
        /*(if (islanding)
        {
            playerState = PlayerState.FLYING_HEIGHTCHANGE;
            movemul += -fallspeed * Time.deltaTime;
        }*/
        toggleTrail(true);
    }
    /*
    void BarrelRoll()
    {
        /*if (playerState == PlayerState.FLYING_BARREL_LEFT)
        {
            roll += turnSpeed * 12 * Time.deltaTime;
            BarrelRollAngle += turnSpeed * 12 * Time.deltaTime;
            if (BarrelRollAngle > 180)
            {
                rolloffsetX -= 0.05f;
            }
            else
            {
                rolloffsetX += 0.05f;
            }
        }
        else if (playerState == PlayerState.FLYING_BARREL_RIGHT)
        {
            roll -= turnSpeed * 12 * Time.deltaTime;
            BarrelRollAngle -= turnSpeed * 12 * Time.deltaTime;
            if ( BarrelRollAngle < -180)
            {
                rolloffsetX += 0.05f;
            }
            else
            {
                rolloffsetX -= 0.05f;   
            }
        }
       
        m_Movement = (verticalmovement * vec3front) + (horizontalmovement * vec3right);
        m_Movement.x += rolloffsetX;
        
        Orientation.Set(pitch, yaw, roll);
        m_Rotation = Quaternion.Euler(Orientation);
        m_Rotation = Quaternion.RotateTowards(m_Rigidbody.rotation, m_Rotation, turnSpeed * 12 * Time.deltaTime);
        m_Movement = m_Rotation * m_Movement;
        m_Rigidbody.AddForce(m_Movement * 20 * Time.deltaTime, ForceMode.VelocityChange);
        m_Rigidbody.MoveRotation(m_Rotation);

        if (BarrelRollAngle >= 360 && playerState == PlayerState.FLYING_BARREL_LEFT)
        {
            playerState = PlayerState.FLYING;
            rolloffsetX = 0;
            BarrelRollAngle = 0;
            m_Rigidbody.angularVelocity = Vector3.zero;
            if (roll > 360)
                roll -= 360;
        }
        else if (BarrelRollAngle <= -360 && playerState == PlayerState.FLYING_BARREL_RIGHT)
        {
            playerState = PlayerState.FLYING;
            rolloffsetX = 0;
            BarrelRollAngle = 0;
            m_Rigidbody.angularVelocity = Vector3.zero;
            if (roll < -360)
                roll += 360;
        }

        if (playerState == PlayerState.FLYING_BARREL_LEFT)
        {
            BarrelRollAngle += BarrelRollSpeed * Time.deltaTime;
            roll += BarrelRollSpeed * Time.deltaTime;
            Orientation.Set(pitch, yaw, roll);
            //m_Rotation = Quaternion.Euler(pitch,yaw,roll);
            //transform.rotation = m_Rotation;
            transform.rotation = Quaternion.AngleAxis(roll,Vector3.right) * Quaternion.AngleAxis(pitch,Vector3.forward) * Quaternion.AngleAxis(yaw,Vector3.up);
            //m_Rigidbody.MoveRotation(m_Rotation);
            m_Rigidbody.MoveRotation(m_Rotation);
            if (BarrelRollAngle >= 360)
            {
                playerState = PlayerState.FLYING;
                rolloffsetX = 0;
                BarrelRollAngle = 0;
                m_Rigidbody.angularVelocity = Vector3.zero;
                if (roll >= 360)
                    roll -= 360;
            }    
        }
    }*/


    void recenterRoll()
    {
        if (roll >= -0.5f && roll <= 0.5)
        {
            roll = 0f;
        }
        else if (roll > 0.5f)
        {
            roll -= Time.deltaTime * flyhorizontalforce *2;
        }
        else if (roll < -0.5f)
        {
            roll += Time.deltaTime * flyhorizontalforce * 2;
        }
    }
    void recenterPitch()
    {
        if (pitch >= -0.5f && pitch <= 0.5f)
        {
            pitch = 0;
        }
        else if (pitch < -0.5)
        {
            pitch += Time.deltaTime * flyverticalforce * 0.5f;
        }
        else if (pitch > 0.5)
            pitch -= Time.deltaTime * flyverticalforce * 0.5f;
    }

    public float getYaw()
    {
        return yaw;
    }

    public void toggleTrail(bool toggle)
    {
        LeftWing.SetActive(toggle);
        RightWing.SetActive(toggle);
    }
}
