using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MovePlayer : MonoBehaviour
{
    [Header("Player Settings")]
    public float turnSpeed = 20f;
    public float airmul = 0.4f;
    public float movemul = 10f;
    public float jumpforce = 10f;
    public float flyverticalforce = 10f;
    public float flymaxIncrease = 2f;
    public float flymaxDecrease = 2f;

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
    bool tookoff = false;

    // double jump
    double firsttaptime=0.0;
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
    Vector3 flyDirection = Vector3.zero;
    Vector3 Orientation = Vector3.zero;

    [Header("Debug")]
    public Text directiontext;
    public Text orientationtext;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        playerheight = transform.localScale.y;
        DefaultMoveMul = movemul;
        vec3up = new Vector3(0, 1, 0);
        flydrop.Set(0,-7.0f, 0);

        photonView = GetComponent<PhotonView>();
        if (photonView == null)
            Debug.Log("Player does not have PhotonView");
        else
        {
            if (!photonView.IsMine)
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }
    private void Start()
    {
    }

    private void Update()
    {
        if (photonView != null)
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
        if (Input.GetKeyUp(KeyCode.C))
        {
            camera.m_LookAt = transform;
            camera.m_Follow = transform;
        }
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
      //  if (photonView.IsMine == true)
        //{
            horizontalmovement = Input.GetAxisRaw("Horizontal");
            verticalmovement = Input.GetAxisRaw("Vertical");
            bool hasHorizontalInput = !Mathf.Approximately(horizontalmovement, 0f);
            bool hasVerticalInput = !Mathf.Approximately(verticalmovement, 0f);
            inputed= hasHorizontalInput || hasVerticalInput;
        //}
    }

    private void FixedUpdate()
    {
        if (photonView != null)
            if (!photonView.IsMine)
                return;
        isgrounded = Physics.Raycast(transform.position, Vector3.down,1f);

        if (isgrounded)
        {
            isFlying = false;
            pitch = 0;
            roll = 0;
            vec3up = Vector3.up;
        }
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
                    axis.Set(1, 0, 0);
                    flyDirection = Quaternion.AngleAxis(-45,axis)* Vector3.Cross(vec3right,vec3up);
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

    void Moveplayer()
    {
        float angle = 0.0f;
        Quaternion forceRotation;
        Debug.Log("cur Photon View is " + photonView.ViewID);
        vec3front = (transform.position - camera.transform.position).normalized;
        vec3front.y = 0;
        vec3front.Normalize();

        vec3right= Vector3.Cross(vec3front,vec3up) *-1;
        vec3view = Vector3.Cross(vec3right, vec3up);
        if (inputed && !isFlying)      //&& !isFlying)// && photonView.IsMine)
        {
            m_Rigidbody.angularVelocity = Vector3.zero;
            m_Movement = ((vec3front * verticalmovement) + (vec3right * horizontalmovement).normalized);
            m_Rigidbody.AddForce((m_Movement * Time.deltaTime) * movemul, ForceMode.VelocityChange);
            angle = Mathf.Rad2Deg * Mathf.Atan2(m_Movement.x, m_Movement.z);
            m_Rotation = Quaternion.AngleAxis(angle, Vector3.up);
            forceRotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * turnSpeed);
            m_Rigidbody.MoveRotation(forceRotation);
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
        else if (collision.gameObject.CompareTag("Interacted"))
        {
            Physics.IgnoreCollision(collision.collider,GetComponent<Collider>(),true);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        
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


        if (isFlying)
        {
            directiontext.text = "Flying: " + flyDirection.ToString();
        }
        else
            directiontext.text = "Grounded: " + vec3front.ToString();

        float angle = 0.0f;
        Quaternion forceRotation;
        vec3front = (transform.position - camera.transform.position).normalized;
        vec3front.Normalize();
        vec3front.y = 0;

        vec3right = Vector3.Cross(vec3front, vec3up) * -1;
        vec3view = Vector3.Cross(vec3right, vec3up);

        m_Rigidbody.angularVelocity = Vector3.zero;
        m_Movement = ((vec3front * verticalmovement) + (vec3right * horizontalmovement).normalized);
        if (Input.GetKey(KeyCode.Space))
        {
            m_Movement.y += Time.deltaTime * flyverticalforce;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_Movement.y -= Time.deltaTime * flyverticalforce;
        }

        m_Rigidbody.AddForce((m_Movement * Time.deltaTime) * movemul, ForceMode.VelocityChange);
        angle = Mathf.Rad2Deg * Mathf.Atan2(vec3front.x, vec3front.z);
        m_Rotation = Quaternion.AngleAxis(angle, Vector3.up);
        forceRotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * turnSpeed);
        m_Rigidbody.MoveRotation(forceRotation);
    }

    public void OnPhotonSerielizedView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
