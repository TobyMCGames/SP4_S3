using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchItemScript : MonoBehaviour
{
    public LineRenderer lineRenderer = null;
    public Vector3 LineDirection;
    Vector3 Endpoint = Vector3.zero;
    public float LineLength = 1;
    public bool IsRendered = true;
    public void Start()
    {
        if (lineRenderer == null)
            Debug.Log("FetchItem line renderer null");

        
    }
    public void Update()
    {
        Endpoint = gameObject.transform.position;
        Endpoint += (LineDirection * LineLength);
        lineRenderer.SetPosition(0, gameObject.transform.position);
        lineRenderer.SetPosition(1, Endpoint);
    }

    public void stoprender(bool toggle)
    {
        lineRenderer.forceRenderingOff = toggle;
    }
}
