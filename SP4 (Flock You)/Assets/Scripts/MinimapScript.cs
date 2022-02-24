using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MinimapScript : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] Cinemachine.CinemachineFreeLook tpc;
    [SerializeField] float ScrollSpeed = 10f;
    float cameraZoom = 10f;

    Camera cam;
    Vector3 minimapPos;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = cameraZoom;
    }

    // Update is called once per frame  
    void LateUpdate()
    {
        minimapPos = Player.transform.position;
        minimapPos.y = Player.transform.position.y + cameraZoom;
        transform.position = minimapPos;
        transform.rotation = Quaternion.Euler(90, tpc.m_XAxis.Value, 0);
        cameraZoom += Input.mouseScrollDelta.y * ScrollSpeed * Time.deltaTime;
    }

    public void setPlayertoFollow(GameObject _player)
    {
        Player = _player;
    }

    public void settpc(CinemachineFreeLook cam)
    {
        tpc = cam;
    }
}
