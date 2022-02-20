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
        JUMPING,
        FLYING,
        FLYING_HEIGHTCHANGE,// when bird is flying up / down
        FLYING_DIVEBOMB,
        FLYING_HOVERING,
    }

    public enum doubletapchecktype
    {
        DOUBLE_SPACE,
        DOUBLE_A,
        DOUBLE_D,
        DOUBLE_NONE,
    }


    [Header("Player Settings")]
    [Tooltip("how fast can the player rotate")]
    public float turnSpeed = 20f;[Tooltip("Movement speed multiplier that affects the player's movementspeed")]
    public float movemul = 10f;[Tooltip("how high the player can jump")]
    public float jumpforce = 10f;
    public float flymaxIncrease = 2f;
    public float flymaxDecrease = 2f;[Tooltip("Determines how fast the bird rolls")]
    public float flyverticalforce = 100f;[Tooltip("Determines how fast the bird pitches")]
    public float flyhorizontalforce = 100f;[Tooltip("Determines how close the player can be before bird prepares to land")]
    public float landingheight = 4f;[Tooltip("how fast the player lands does not take into account player height")]
    public float landingtimer = 0.1f;
    public float birdterminalvelocity = -30f;
    public PlayerState playerState = PlayerState.IDLE;
    private float landingelapsed = 0.0f;
    private float divebombminpitch = 90.0f;
    private float fallspeed = 0f;
    

    private PhotonView photonView;

    [Header("Player Camera")]
    public Cinemachine.CinemachineFreeLook camera;
    public GameObject Lookat;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    float horizontalmovement;
    float verticalmovement;
    float groundrag = 6f;
    float airDrag = 2f;
    float DefaultMoveMul;
    float playerheight;

    bool inputed = false;
    bool isgrounded = false;
    bool islanding = false;

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
    float diveyaw = 0f;

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

    private doubletapchecktype doublechecktype = doubletapchecktype.DOUBLE_NONE;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        playerheight = transform.localScale.y;
        DefaultMoveMul = movemul;
        vec3up = new Vector3(0, 1, 0);
        flydrop.Set(0, -3.0f, 0);
        DiveBombDirection = Physics.gravity;
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
            Debug.Log("Player does not have PhotonView");
        else
        {
            if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.MULTIPLAYER)
            {
                if (!photonView.IsMine)
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }
        
    }
    private void Start()
    {
        pitch = baserotation.x;
        yaw = baserotation.y;
        roll = baserotation.z;
        m_Rigidbody.MoveRotation(Quaternion.Euler(pitch, yaw, roll));
    }

    private void Update()
    {
        if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;
        PlayerInput();
        controlDrag();
        Moveplayer();
        if (playerState >= PlayerState.FLYING)
        {
            m_Rigidbody.useGravity = false;
            Fly();
        }
        else
            m_Rigidbody.useGravity = true;
        //checkdoubletap();
        checkJump(); // checks if player jumps
        if (Input.GetKeyUp(KeyCode.C))
        {
            camera.m_LookAt = transform;
            camera.m_Follow = transform;
        }
        landingcheck();
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
            else if (!isgrounded && playerState < PlayerState.FLYING && playerState != PlayerState.FLYING_HOVERING)
            {
                playerState = PlayerState.FLYING;
                //flydrop =
                if (flydrop.y > 0)
                    flydrop *= -1;
            }
        }
    }

    void PlayerInput()
    {
            horizontalmovement = Input.GetAxisRaw("Horizontal");
            verticalmovement = Input.GetAxisRaw("Vertical");
            bool hasHorizontalInput = !Mathf.Approximately(horizontalmovement, 0f);
            bool hasVerticalInput = !Mathf.Approximately(verticalmovement, 0f);
            inputed= hasHorizontalInput || hasVerticalInput;
    }

    private void FixedUpdate()
    {
        if (FYGameManager.Instance.curMode == FYGameManager.GAMEMODES.MULTIPLAYER)
            if (!photonView.IsMine)
                return;
        // check player grounded

        isgrounded = Physics.Raycast(m_Rigidbody.position, Vector3.down, out RaycastHit hit, playerheight * 0.5f - fallspeed * Time.deltaTime);

        if (isgrounded)
        {
            pitch = 0;
            roll = 0;
            vec3up = Vector3.up;
            
            diveyaw = 0;
            if (playerState == PlayerState.FLYING_DIVEBOMB)
                m_Rigidbody.position = hit.point + (Vector3.up * playerheight * 0.5f);
            playerState = PlayerState.IDLE;
            fallspeed = 0;
            m_Rigidbody.useGravity = true;
        }

        if (directiontext != null)
        {
            if (playerState == PlayerState.FLYING)
            {
                directiontext.text = "Flying: " + Orientation.ToString();
            }
            else
                directiontext.text = "Grounded: " + vec3front.ToString();
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
            m_Rigidbody.drag = groundrag;
        else if (!isgrounded)
            m_Rigidbody.drag = airDrag;
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
            m_Rigidbody.useGravity = true;
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
        checkdoubletap();
        if (playerState == PlayerState.FLYING_DIVEBOMB)
        {
            divebomb();
            return;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            //m_Movement.y += Time.deltaTime * flyverticalforce;
            playerState = PlayerState.FLYING_HEIGHTCHANGE;
            if (!inputed) // striaght up
                m_Rigidbody.AddForce(Quaternion.Euler(0, 90, 0) * (Vector3.up * flyverticalforce), ForceMode.Acceleration);

            else // go up a relative to your orientation
            {
                Vector3 movement = Orientation;
                movement.y = 0;
                m_Rigidbody.AddForce(Quaternion.Euler(Orientation) *
                    (Vector3.up * flyverticalforce * Time.deltaTime), ForceMode.VelocityChange);

                if (pitch - Time.deltaTime * flyverticalforce < -45)
                {
                    pitch = -45;
                }
                else
                    pitch -= Time.deltaTime * flyverticalforce;
            }
        }
        else if (!islanding && playerState != PlayerState.FLYING_DIVEBOMB)
        {
            recenterPitch();
            playerState = PlayerState.FLYING;
        }


        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerState = PlayerState.FLYING_HEIGHTCHANGE;
            if (!inputed) // fly striaght down;
            {
                m_Rigidbody.AddForce(Quaternion.Euler(0, 90, 0) * (flyverticalforce * Vector3.down), ForceMode.Acceleration);
            }
            else // go down a relative to your orientation
            {
                recenterRoll();
                playerState = PlayerState.FLYING;
                Vector3 movement = Orientation;
                movement.y = 0;
                m_Rigidbody.AddForce(Quaternion.Euler(Orientation) * Vector3.down *
                    flyverticalforce * Time.deltaTime, ForceMode.VelocityChange);

                if (pitch + Time.deltaTime * flyverticalforce > 45)
                    pitch = 45;
                else
                    pitch += Time.deltaTime * flyverticalforce;
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
            rbRotation = Quaternion.Lerp(m_Rigidbody.rotation, m_Rotation, Time.deltaTime * turnSpeed);
            m_Rigidbody.AddForce((m_Movement * Time.deltaTime) * movemul, ForceMode.VelocityChange);
            m_Rigidbody.MoveRotation(rbRotation);
        }
        else
        {
            if (playerState != PlayerState.FLYING_HEIGHTCHANGE)
            {
                simulateflap();
            }
            Orientation.Set(pitch, yaw, roll);
            m_Rotation = Quaternion.Euler(Orientation);
            rbRotation = Quaternion.Lerp(m_Rigidbody.rotation, m_Rotation, Time.deltaTime * turnSpeed);
            m_Rigidbody.MoveRotation(rbRotation);
        }
    }

    public void checkdoubletap() //doubletapchecktype doubletap
    { //&& doublechecktype != doubletapchecktype.DOUBLE_SPACE
        if (doubletapcheck )
        {
            elapsedtaptime += Time.deltaTime;
            if (elapsedtaptime > doubletaptimeframe)
            {
                doubletapcheck = false;
                elapsedtaptime = 0;
                Debug.Log("single tap");
            }
        }
        if (Input.GetButtonUp("Jump") && playerState > PlayerState.FLYING)
        {
            if (doubletapcheck)
            {
                if (Time.time - firsttaptime < doubletaptimeframe)
                {
                    doubletapcheck = false;
                    elapsedtaptime = 0;
                    Orientation.Set(0, 0, 0);
                    if (playerState > PlayerState.FLYING && playerState != PlayerState.FLYING_DIVEBOMB)
                    {
                        playerState = PlayerState.FLYING_DIVEBOMB;
                        DiveBombDirection = vec3front;
                    }
                    else if (playerState == PlayerState.FLYING_DIVEBOMB) // recover divebomb
                    {

                    }
                }
                else
                {
                    doubletapcheck = false;
                    Debug.Log("single tap");
                }
            }
            else
            {
                firsttaptime = Time.time;
                doubletapcheck = true;
                doublechecktype = doubletapchecktype.DOUBLE_SPACE;
            }
        }
    }

    /*public void checkdoubletap(doubletapchecktype doubletap)
    {
        switch (doubletap)
        {
            case doubletapchecktype.DOUBLE_SPACE:
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    if (!doubletapcheck)
                    {
                        doubletapcheck = true;
                        firsttaptime = Time.time;
                    }
                }
                else if (doubletapcheck && Input.GetKey(KeyCode.Space)) // divebomb
                {

                }
                break;
            case doubletapchecktype.DOUBLE_A:
                if (Input.GetKeyUp(KeyCode.A))
                {
                    if (!doubletapcheck)
                    {
                        doubletapcheck = true;
                        firsttaptime = Time.time;
                    }
                }
                break;
            case doubletapchecktype.DOUBLE_D:
                if (Input.GetKeyUp(KeyCode.D))
                {
                    if (!doubletapcheck)
                    {
                        doubletapcheck = true;
                        firsttaptime = Time.time;
                    }
                }
                break;
        }
        if (Time.time - firsttaptime < doubletaptimeframe)
        {
            doubletapcheck = false;
            elapsedtaptime = 0;
            Orientation.Set(0, 0, 0);
            if (playerState > PlayerState.FLYING && playerState != PlayerState.FLYING_DIVEBOMB)
            {
                playerState = PlayerState.FLYING_DIVEBOMB;
                DiveBombDirection = vec3front;
            }
            else if (playerState == PlayerState.FLYING_DIVEBOMB) // recover divebomb
            {

            }
        }
        else
        {
            doubletapcheck = false;
            Debug.Log("single tap");
        }

    }*/


    void landingcheck()
    {
        islanding = Physics.Raycast(m_Rigidbody.position, Vector3.down,landingheight+fallspeed);
        //Debug.Log(islanding);
    }

    void simulateflap()
    {
        if (currenflaptimer >= flaptimermax)
        {
            m_Rigidbody.AddForce(flydrop,ForceMode.Impulse);
            // stop camera from going up and down
            Lookat.transform.localPosition = (flydrop / GetComponent<Rigidbody>().mass *Time.deltaTime) *-1;
            currenflaptimer = 0;
            flydrop *= -1;
        }
        else
            currenflaptimer += Time.deltaTime;
    }

    public void ChangePlayerState(PlayerState nextplayerState)
    {
        switch (playerState) // check what is the current state and change the values according to what is the next state
        {
            case PlayerState.IDLE:
                break;
            case PlayerState.JUMPING:
                break;
            case PlayerState.FLYING:
                break;
            
            case PlayerState.FLYING_DIVEBOMB:
                break;
        }
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
        
        yaw = Mathf.Rad2Deg * Mathf.Atan2(vec3front.x,vec3front.z);
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
            fallspeed += -0.1f;
        landingcheck();
        handleLanding();
    }
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
}
