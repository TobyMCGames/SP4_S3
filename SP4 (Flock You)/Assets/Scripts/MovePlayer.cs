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
    bool inputed = false;

    // temp values 
    public float jumpforce = 10f;
    float playerheight;
    public Cinemachine.CinemachineFreeLook camera;
    bool isFlying = false;


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
    
        if (Input.GetKeyUp(KeyCode.C))
        {
            camera.m_LookAt = transform;
        }
    }


    void Jump()
    {
        m_Rigidbody.AddForce(transform.up * jumpforce, ForceMode.Impulse);
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
    }
    void Moveplayer()
    {
        float angle = 0.0f;
        Quaternion forceRotation;
        Vector3 directionforward = (transform.position - camera.transform.position).normalized;
        directionforward.y = 0;
        directionforward.Normalize();

        Vector3 directionright = Vector3.Cross(directionforward, Vector3.up) *-1;

        if (inputed == true && !isFlying)// && photonView.IsMine)
        {
            m_Rigidbody.angularVelocity = Vector3.zero;
            m_Movement = ((directionforward * verticalmovement) + (directionright * horizontalmovement).normalized);
            m_Movement.Normalize();
            m_Rigidbody.AddForce((m_Movement * Time.deltaTime) * movemul, ForceMode.VelocityChange);
            angle = Mathf.Rad2Deg * Mathf.Atan2(m_Movement.x, m_Movement.z);
            //Debug.Log(angle);
            m_Rotation = Quaternion.AngleAxis(angle, Vector3.up);
            forceRotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * turnSpeed);
            m_Rigidbody.MoveRotation(forceRotation);
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
        }
    }
}
