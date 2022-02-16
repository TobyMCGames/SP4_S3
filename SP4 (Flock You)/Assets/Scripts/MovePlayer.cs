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
    double doubletaptimeframe = 0.7;
    float elapsedtaptime = 0f;

    Vector3 vec3right = Vector3.zero;
    Vector3 vec3up = Vector3.zero;
    Vector3 vec3front = Vector3.zero;
    Vector3 flyDirection = Vector3.zero;

    float flyup = 0.0f;

    float pitch = 0.0f, yaw = 0.0f, roll = 0.0f;


    [Header("Debug")]
    public Text directiontext;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        playerheight = transform.localScale.y;
        DefaultMoveMul = movemul;
        vec3up = new Vector3(0, 1, 0);
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
            directiontext.text = "Flying: " + flyDirection.ToString();
        }
        else
            directiontext.text = flyDirection.ToString();
        if (isgrounded)
        {
            isFlying = false;
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
                    flyDirection = vec3front;
                    flyDirection.y += 0.5f;
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

        vec3right= Vector3.Cross(vec3front, Vector3.up) *-1;
        if (inputed && !isFlying)      //&& !isFlying)// && photonView.IsMine)
        {
            m_Rigidbody.angularVelocity = Vector3.zero;
            m_Movement = ((vec3front * verticalmovement) + (vec3right * horizontalmovement).normalized);
            m_Movement.Normalize();
            m_Rigidbody.AddForce((m_Movement * Time.deltaTime) * movemul, ForceMode.VelocityChange);
            angle = Mathf.Rad2Deg * Mathf.Atan2(m_Movement.x, m_Movement.z);
            m_Rotation = Quaternion.AngleAxis(angle, Vector3.up);
            forceRotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * turnSpeed);
            m_Rigidbody.MoveRotation(forceRotation);
            Vector3.RotateTowards(vec3front, forceRotation.eulerAngles, Mathf.PI * 2, 0.0f);
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
        /*else if (collision.gameObject.CompareTag("Interacted"))
        {
            Physics.IgnoreCollision(collision.collider,GetComponent<Collider>(),true);
        }*/
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
        /*if (tookoff == false)
        {

        }*/
        if (Input.GetKey(KeyCode.Space))
        {
             flyup += flyverticalforce * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            flyup -= flyverticalforce * Time.deltaTime;
        }
        if ( flyup > 1)
        {
            flyup = 1;
        }
        else if (flyup < -1)
        {
            flyup = -1;
        }
        flyDirection.y = flyup;

        vec3front = transform.position - camera.transform.position;
        vec3front.Normalize();
        vec3front.y = 0;
        //change later due to different orientation
        vec3right = Vector3.Cross(vec3front,vec3up) *-1;

      
        if (verticalmovement > 0)
        {
            flyDirection = (vec3front * verticalmovement + vec3right * horizontalmovement);
        }

        flyDirection.Normalize();
        flyDirection.y = flyup;
        //  Debug.Log(flyDirection);
        m_Rigidbody.AddForce(flyDirection * 30f, ForceMode.Acceleration);

        // rotation of flying force 
        
        if (horizontalmovement == 0)
        {
            float angle = Mathf.Rad2Deg * Mathf.Atan2(vec3front.x, vec3front.z);
            // make this relative to player's up
            m_Rotation = Quaternion.AngleAxis(angle, vec3up);

            m_Rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * turnSpeed));
        }
    }
}
