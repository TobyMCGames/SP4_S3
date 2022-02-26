using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MinimapScript : MonoBehaviour
{
    [SerializeField] GameObject Player = null;
    [SerializeField] CinemachineFreeLook tpc;
    [SerializeField] float BasecameraOffset = 20f;
    [SerializeField] float ScrollSpeed = 10f;
    [SerializeField] float minZoom = 5f;
    [SerializeField] float currZoom = 10f;
    [SerializeField] float maxZoom = 20f;
    [SerializeField] float zoomspeed = 5f;

    Camera cam;
    Vector3 minimapPos;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = currZoom;
        
    }

    // Update is called once per frame  
    private void Update()
    {
        if (FYGameManager.Instance.isOver() && Player != null)
            return;

        if (Player == null)
            return;

        minimapPos = Player.transform.position;
        minimapPos.y = Player.transform.position.y + BasecameraOffset;
        transform.position = minimapPos;
        transform.rotation = Quaternion.Euler(90, tpc.m_XAxis.Value, 0);

        currZoom -= Input.mouseScrollDelta.y * ScrollSpeed * Time.deltaTime;

        if (currZoom > maxZoom)
            currZoom = maxZoom;
        if (currZoom < minZoom)
            currZoom = minZoom;
        cam.orthographicSize = currZoom;
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
