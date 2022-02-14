using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    public enum CAMERA_TYPE
    {
        FIRST_PERSON,
        THIRD_PERSON,
        NUM_CAMERA_TYPE
    }

    public float turnSpeed = 20f;

    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private AudioSource m_AudioSource;
    private Vector3 m_Movement;
    private Quaternion m_Rotation = Quaternion.identity;
    private PhotonView photonView;
    private float fmovementSpeed = 4f;
    private GameObject _PointInView;
    private GameObject _Camera;
    private CAMERA_TYPE type;
    private PlayerInfo playerInfo;

    void Awake ()
    {
        photonView = GetComponent<PhotonView>();
        m_Animator = GetComponent<Animator> ();
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_AudioSource = GetComponent<AudioSource> ();
        playerInfo = GetComponent<PlayerInfo>();

        if (photonView.IsMine)
            GetComponent<AudioListener>().enabled = true;

        _PointInView = gameObject.transform.Find("PointOfView").gameObject;
    }

    void FixedUpdate ()
    {
        if (!photonView.IsMine)
            return;

        // Calculate angle for movement direction
        Vector3 displacement = _Camera.transform.position - gameObject.transform.position;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(displacement.x, displacement.z);
        if (type == CAMERA_TYPE.THIRD_PERSON)
            angle -= 180.0f;

        m_Rotation = Quaternion.AngleAxis(angle, Vector3.up);

        // Check for input
        float horizontal = 0.0f;
        if (playerInfo.GetTeam() == PlayerInfo.TEAMS.PROP)
            horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis ("Vertical");
        
        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize ();
        if (_Camera != null)
        {
            m_Movement = m_Rotation * m_Movement;
        }

        bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        
        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop ();
        }

        if (m_Animator == null || !m_Animator.enabled)
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * fmovementSpeed * Time.deltaTime);
            _PointInView.transform.position = m_Rotation * Vector3.forward + gameObject.transform.position;
        }
        else
        {
            m_Animator.SetBool("IsWalking", isWalking);
        }
    }

    void OnAnimatorMove ()
    {
        if (!photonView.IsMine && m_Animator == null)
            return;
      
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * fmovementSpeed * m_Animator.deltaPosition.magnitude);
        _PointInView.transform.position = m_Rotation * Vector3.forward + gameObject.transform.position;
        _PointInView.transform.rotation = m_Rotation;
    }

    public void SetCamera(GameObject _camera, CAMERA_TYPE type)
    {
        this._Camera = _camera;
        this.type = type;
    }

    public GameObject GetCamera()
    {
        return _Camera;
    }
}