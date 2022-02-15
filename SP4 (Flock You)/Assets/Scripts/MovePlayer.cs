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
    public float airmul = 0.4f;
    bool isWalking;

    // temp values 
    float tempjumpforce = 10f;
    float playerheight;
    float movespeed = 60f;
    public GameObject camera;

    bool isFlying = false;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //if (photonView.IsMine)
        //GetComponent<AudioListener>().enabled = true;
        playerheight = transform.localScale.y;
        
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
        Quaternion targetrotation;
        if (isWalking == true)// && photonView.IsMine)
        {
            Vector3 player_camera = (camera.transform.position - transform.position);

            angle = Mathf.Rad2Deg * Mathf.Atan2(player_camera.x, player_camera.z);
            angle -= 180;

            m_Rotation = Quaternion.AngleAxis(angle, Vector3.up);
            targetrotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * 10f);
            m_Movement = (transform.forward * verticalmovement) + (transform.right * horizontalmovement);
            if (isgrounded)
            {
                m_Rigidbody.AddForce(m_Movement.normalized * movespeed, ForceMode.Acceleration);
            } 
            else if (!isgrounded)
            {
                m_Rigidbody.AddForce(m_Movement.normalized * movespeed* airmul, ForceMode.Acceleration);
            }
            
            // rotate the player towards where the camera is facing
            if (verticalmovement != 0)
            {
                
                m_Rigidbody.MoveRotation(targetrotation);
            }
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
}
