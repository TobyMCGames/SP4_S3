using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSystem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float ForcerequiredtoBreak;
    [SerializeField] GameObject interactlist;
    private Rigidbody m_rigidbody;
    bool isbroken = false;
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }



    private void Start()
    {
        InteractItem interactscript;
        foreach (Transform t in transform)
        {
            if (interactscript = t.gameObject.GetComponent<InteractItem>())
            {
                interactscript.InteractableList = interactlist.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isbroken == true)
            return;
        if (m_rigidbody.velocity.sqrMagnitude >= ForcerequiredtoBreak)
        {
            Destroy(gameObject);
            isbroken = true;
        }
    }


    public void ReleaseInventory()
    { 
        foreach (Transform t in transform)
        {
             t.SetParent(interactlist.transform);
            t.gameObject.SetActive(true);
        }
    }
    private void OnDestroy()
    {
        ReleaseInventory();
    }
}
