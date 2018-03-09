using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ocu_stick : MonoBehaviour {

    public GameObject thumbstick;
    private float forwardValue;
    private float turnValue;
    private Quaternion defaultRotation;

    // Use this for initialization
    void Start()
    {
        defaultRotation = thumbstick.transform.localRotation;

    }

    // Update is called once per frame
    void Update()
    {
        thumbstick.transform.localRotation = defaultRotation;
        forwardValue = Input.GetAxis("Vertical");
        turnValue = Input.GetAxis("Horizontal");

        // Calculate a transform
        thumbstick.transform.Rotate(Vector3.right, -30 * forwardValue);
        thumbstick.transform.Rotate(Vector3.up, -30 * turnValue);

        
    }
}
