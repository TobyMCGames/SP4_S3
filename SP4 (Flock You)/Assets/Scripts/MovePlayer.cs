using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MovePlayer : MonoBehaviour
{
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
    private float landingelapsed = 0.0f;

    private PhotonView photonView;

    [Header("Player Camera")]
    public Cinemachine.CinemachineFreeLook camera;

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
    bool isgrounded;
    bool isFlying = false;
    bool islanding = false;

    // double jump
    double firsttaptime = 0.0;
    bool doubletapcheck = false;
    double doubletaptimeframe = 0.5f;
    float elapsedtaptime = 0f;


    [Header("Flysettings")]
    Vector3 flydrop = Vector3.zero;
    Vector3 flyup = Vector3.zero;
    float flaptimermax = 0.1f;
    float currenflaptimer = 0.0f;

    [Header("Orientation")]
    float pitch = 0.0f, yaw = 0.0f, roll = 0.0f;
    Vector3 vec3right = Vector3.zero;
    Vector3 vec3up = Vector3.zero;
    Vector3 vec3front = Vector3.zero;
    Vector3 vec3view = Vector3.zero;
    Vector3 flyDirectionfront = Vector3.zero;
    Vector3 flyDirectionright = Vector3.zero;
    Vector3 Orientation = Vector3.zero;
    public Vector3 baserotation = Vector3.zero;

    [Header("Debug")]
    public Text directiontext;
    public Text orientationtext;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        playerheight = transform.localScale.y;
        DefaultMoveMul = movemul;
        vec3up = new Vector3(0, 1, 0);
        flydrop.Set(0, -7.0f, 0);

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
        if (isFlying)
        {
            m_Rigidbody.useGravity = false;
            Fly();
        }
        else
            m_Rigidbody.useGravity = true;
        checkdoubletap();
        if (Input.GetKeyUp(KeyCode.C))
        {
            camera.m_LookAt = transform;
            camera.m_Follow = transform;
        }
        landingcheck();
    }

    void Jump()
    {
        if (isgrounded)
        {
            m_Rigidbody.AddForce(transform.up * jumpforce, ForceMode.Impulse);
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
        isgrounded = Physics.Raycast(m_Rigidbody.position, Vector3.down, 1f);

        if (isgrounded)
        {
            isFlying = false;
            pitch = 0;
            roll = 0;
            vec3up = Vector3.up;
        }

        if (directiontext != null)
        {
            if (isFlying)
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
        vec3view = Vector3.Cross(vec3right, vec3up);
        if (inputed && !isFlying)      //&& !isFlying)// && photonView.IsMine)
        {
            m_Rigidbody.angularVelocity = Vector3.zero;
            m_Movement = ((vec3front * verticalmovement) + (vec3right * horizontalmovement)).normalized;
            m_Rigidbody.AddForce((m_Movement * Time.deltaTime) * movemul, ForceMode.VelocityChange);
            yaw = Mathf.Rad2Deg * Mathf.Atan2(m_Movement.x, m_Movement.z);
            m_Rotation = Quaternion.AngleAxis(yaw, Vector3.up);
            rbRotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * turnSpeed);
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
            isFlying = false;
            m_Rigidbody.useGravity = true;
        }
    }

    public void singletapJump()
    {
        if (isgrounded)
        {
            Jump();
        }
    }

    public void Fly()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            //m_Movement.y += Time.deltaTime * flyverticalforce;
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
        else if (!islanding)
        {
            if (pitch < 0.5)
            {
                pitch += Time.deltaTime * flyverticalforce * 0.5f;
            }
            else if (pitch > 0.5)
                pitch -= Time.deltaTime * flyverticalforce * 0.5f;

        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            // m_Movement.y -= Time.deltaTime * flyverticalforce;
            if (!inputed) // fly striaght down;
            {
                m_Rigidbody.AddForce(Quaternion.Euler(0, 90, 0) * (flyverticalforce * Vector3.down), ForceMode.Acceleration);
            }
            else // go down a relative to your orientation
            {
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
        else if (pitch > 0.5 && !islanding)
        {
            pitch -= Time.deltaTime * flyverticalforce;
        }

        Quaternion rbRotation;
        vec3front = (m_Rigidbody.position - camera.transform.position).normalized;
        vec3front.Normalize();
        vec3front.y = 0;
        vec3right = Vector3.Cross(vec3front, vec3up) * -1;
        vec3view = Vector3.Cross(vec3right, vec3up);

        m_Rigidbody.angularVelocity = Vector3.zero;

        if (!islanding)
        {
            if (horizontalmovement != 0)
            {
                roll -= (horizontalmovement * flyhorizontalforce * Time.deltaTime);
                if (roll > 45)
                    roll = 45;
                else if (roll < -45)
                    roll = -45;
            }
            else if (roll > 0.5f)
            {
                roll -= Time.deltaTime * flyhorizontalforce * 0.5f;
            }
            else if (roll < 0.5f)
            {
                roll += Time.deltaTime * flyhorizontalforce * 0.5f;
            }
        }


        // current

        // player has inputed then we calculate where we facing
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
            Orientation.Set(pitch, yaw, roll);
            m_Rotation = Quaternion.Euler(Orientation);
            rbRotation = Quaternion.Lerp(m_Rigidbody.rotation, m_Rotation, Time.deltaTime * turnSpeed);
            m_Rigidbody.MoveRotation(rbRotation);
        }
    }

    public void checkdoubletap()
    {
        if (doubletapcheck)
        {
            elapsedtaptime += Time.deltaTime;
            if (elapsedtaptime > doubletaptimeframe)
            {
                singletapJump();
                doubletapcheck = false;
                elapsedtaptime = 0;
                Debug.Log("single tap");
            }
        }
        if (Input.GetButtonUp("Jump"))
        {
            if (doubletapcheck)
            {
                if (Time.time - firsttaptime < doubletaptimeframe)
                {
                    isFlying = !isFlying;
                    doubletapcheck = false;
                    elapsedtaptime = 0;
                    Vector3 axis = Vector3.zero;
                    Orientation.Set(0, 0, 0);
                    axis.Set(1, 0, 0);
                    Debug.Log("double tap");
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
                Jump();
            }
        }
    }


    void landingcheck()
    {
        islanding = Physics.Raycast(m_Rigidbody.position, Vector3.down,landingheight);
        //Debug.Log(islanding);
    }

    void simulateflap()
    {
        if (currenflaptimer >= flaptimermax)
        {

        }
        else
            currenflaptimer += Time.deltaTime;

        

    }
}
