using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throwbot_controller : MonoBehaviour {

    public GameObject LeftWheel;
    public GameObject RightWheel;
    private float forwardValue;
    private float turnValue;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        forwardValue = Input.GetAxis("Vertical");
        turnValue = Input.GetAxis("Horizontal");

        // Move forward
        LeftWheel.transform.Rotate(Vector3.right, 1 * forwardValue, Space.Self);
        RightWheel.transform.Rotate(Vector3.right, 1 * forwardValue, Space.Self);
        transform.Translate(Vector3.forward * forwardValue * Time.deltaTime);

        // Rotate 
        transform.Rotate(Vector3.up, 1 * turnValue, Space.Self);
    }
}
