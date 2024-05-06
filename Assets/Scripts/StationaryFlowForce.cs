using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryFlowForce : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.up * -5.0f, Color.green);
    }
}
