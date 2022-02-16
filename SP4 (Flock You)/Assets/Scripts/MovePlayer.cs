using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float turnSpeed = 20f;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    float horizontalmovement;
    float verticalmovement;
    bool isgrounded;
    float groundrag = 6f;
    float airDrag = 2f;
    public float movemul = 10f;
    float DefaultMoveMul;
    public float airmul = 0.4f;
    bool isWalking;

    // temp values 
    float tempjumpforce = 10f;
    float playerheight;
    float movespeed = 60f;
    public GameObject camera;
    bool isFlying = false;
    public float movementForce = 1;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //if (photonView.IsMine)
        //GetComponent<AudioListener>().enabled = true;
        playerheight = transform.localScale.y;
        DefaultMoveMul = movemul;
        
    }
    private void Start()
    {
    }

    private void Update()
    {
        PlayerInput();
        controlDrag();
        if (Input.GetButtonDown("Jump") && isgrounded == true)// && photonView.IsMine == true)
        {
            Jump();
        }
        Moveplayer();
    }

    void Jump()
    {
        m_Rigidbody.AddForce(transform.up * tempjumpforce, ForceMode.Impulse);
    }

    void PlayerInput()
    {
      //  if (photonView.IsMine == true)
        //{
            horizontalmovement = Input.GetAxisRaw("Horizontal");
            verticalmovement = Input.GetAxisRaw("Vertical");
            bool hasHorizontalInput = !Mathf.Approximately(horizontalmovement, 0f);
            bool hasVerticalInput = !Mathf.Approximately(verticalmovement, 0f);
            isWalking = hasHorizontalInput || hasVerticalInput;
        //}
    }

    private void FixedUpdate()
    {
        isgrounded = Physics.Raycast(transform.position, Vector3.down, playerheight * 0.5f + 0.1f);
        
    }
    void Moveplayer()
    {
        float angle = 0.0f;
        Quaternion forceRotation;
        if (isWalking == true)// && photonView.IsMine)
        {
            Vector3 directionforward = camera.transform.forward;
            directionforward.y = 0;
            directionforward.Normalize();
            Vector3 directionright = camera.transform.right;
            directionright.y = 0;
            directionright.Normalize();
            m_Movement = ((directionforward * verticalmovement) + (directionright * horizontalmovement)).normalized;
     
            Vector3 player_camera = (camera.transform.position - transform.position);
            //angle = Mathf.Rad2Deg * Mathf.Atan2(player_camera.x, player_camera.z) - 180;
            angle = Mathf.Rad2Deg * Mathf.Atan2(m_Movement.z,m_Movement.x) - 180;
            Debug.Log(angle);
            m_Rigidbody.AddForce(m_Movement,ForceMode.VelocityChange);

            // set the direction of the force applied to the rigid body.
            m_Rotation = Quaternion.AngleAxis(angle, Vector3.up);
            forceRotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * turnSpeed);
            m_Rigidbody.MoveRotation(forceRotation);

            // rotate the player towards where the camera is facing
            /* if (isgrounded)
             {
                 Vector3 player_camera = (camera.transform.position - transform.position);
                 if (verticalmovement > 0)
                 {
                     angle = Mathf.Rad2Deg * Mathf.Atan2(player_camera.x, player_camera.z);
                     angle -= 180;
                     m_Rotation = Quaternion.AngleAxis(angle, Vector3.up);
                     targetrotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * 10f);
                     m_Rigidbody.MoveRotation(targetrotation);
                     m_Rigidbody.AddForce(m_Movement.normalized * movespeed, ForceMode.Acceleration);
                 }
                 else if (verticalmovement < 0)
                 {

                     angle = Mathf.Rad2Deg * Mathf.Atan2(player_camera.x, player_camera.z);
                     m_Rotation = Quaternion.AngleAxis(angle, Vector3.up);
                     targetrotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * 10f);
                     m_Rigidbody.MoveRotation(targetrotation);
                     m_Rigidbody.AddForce(-m_Movement.normalized * movespeed, ForceMode.Acceleration);
                 }
             }
             else if (!isgrounded)
             {
                 m_Rigidbody.AddForce(m_Movement.normalized * movespeed * airmul, ForceMode.Acceleration);
             }*/

            /*else
            {
                angle = Mathf.Rad2Deg * Mathf.Atan2(verticalmovement, horizontalmovement) + transform.eulerAngles.y;
                m_Rotation = Quaternion.AngleAxis(angle, Vector3.up);
                targetrotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * 10f);
            }*/

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
}
