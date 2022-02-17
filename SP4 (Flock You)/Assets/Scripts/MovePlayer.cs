using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    double doubletaptimeframe = 0.3f;
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

    //[Header("Debug")]
    //public Text directiontext;
    //public Text orientationtext;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        playerheight = transform.localScale.y;
        DefaultMoveMul = movemul;
        vec3up = new Vector3(0, 1, 0);
        flydrop.Set(0,-7.0f, 0);
    }
    private void Start()
    {
    }

    private void Update()
    {
        PlayerInput();
        controlDrag();
        Moveplayer();
        if (isFlying)
        {
            Fly();
        }
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
        isgrounded = Physics.Raycast(transform.position, Vector3.down, playerheight * 0.5f + 0.1f);
        if (isFlying)
        {
            //directiontext.text = "Flying: " + flyDirection.ToString();
        }
        else
            //directiontext.text = "Grounded: " + vec3front.ToString();
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
                    if (isFlying == true)
                        isFlying = false;
                    else
                        isFlying = true;
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
             flyup.y += flyverticalforce * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            flyup.y -= flyverticalforce * Time.deltaTime;
        }
        if ( flyup.y > 1)
        {
            flyup.y = 1;
        }
        else if (flyup.y < -1)
        {
            flyup.y = -1;
        }

        flyup = Quaternion.AngleAxis(pitch, Vector3.up) * flyup;
        flyDirection.y = flyup.y;
        flydrop.y = flyup.y*2;

        vec3front = transform.position - camera.transform.position;
        vec3front.y = 0;
        vec3front.Normalize();
        //change later due to different orientation
        vec3right = Vector3.Cross(vec3front,vec3up) *-1;

        flyDirection = (vec3front * verticalmovement) + (vec3right * horizontalmovement);
        flyDirection.Normalize();
        flyDirection.y = flyup.y;

        // flapping 
        /*if (currenflaptimer > flaptimermax)
        {
            currenflaptimer = 0f;
            m_Rigidbody.AddForce(flydrop * 50 * Time.deltaTime, ForceMode.Impulse);
        }
        else
        {
            currenflaptimer += Time.deltaTime;
            m_Rigidbody.AddForce(flyDirection * 30f * Time.deltaTime, ForceMode.Impulse);
        }*/



        // rotation of flying force 

        yaw = Mathf.Rad2Deg * Mathf.Atan2(vec3front.x, vec3front.z);
   
        // right 
        if (horizontalmovement != 0)
        {
            flyDirection = Vector3.Cross(vec3right, flyDirection);
            if (roll + horizontalmovement * Time.deltaTime * -10 > 45f)
            {
                roll = 45f;
            }
            else if (roll + horizontalmovement * Time.deltaTime *-10 < -45)
            {
                roll = -45f;
            }
            else
                roll += horizontalmovement * Time.deltaTime * 10 * -1;
        }

        if (verticalmovement != 0)
        {
            if (pitch + verticalmovement *Time.deltaTime * -10 > 45f)
            {
                pitch = 45f;
            }
            else if (pitch + verticalmovement * Time.deltaTime * -10 < -45f)
            {
                pitch = -45f;
            }
            else
            {
                pitch += verticalmovement * Time.deltaTime * -10f;
            }
        }
        Orientation.Set(pitch, yaw, roll);
        // update player vector  
        vec3right = (Quaternion.Euler(Orientation) * vec3right).normalized;
        vec3up = (Quaternion.Euler(Orientation) * vec3up).normalized;
        vec3front = (Vector3.Cross(vec3up, vec3right)).normalized;
        vec3front.y = 0;
        m_Rotation = Quaternion.Euler(Orientation);
        //orientationtext.text = "Orientation: " + Orientation.ToString();
        if (verticalmovement < 0)
        {
            m_Rigidbody.AddForce(flyDirection * -30f, ForceMode.Acceleration);
        }
        else if (verticalmovement > 0)
        {
            m_Rigidbody.AddForce(flyDirection * 30f, ForceMode.Acceleration);
        }
        else // fly striaght
        {
            //flyDirection.y = 0;
            m_Rigidbody.AddForce(flyDirection * 30f, ForceMode.Acceleration);
        }

        //m_Rigidbody.MoveRotation(m_Rotation);
        Quaternion forceRotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * turnSpeed);
        m_Rigidbody.MoveRotation(forceRotation);

        
    }
}
