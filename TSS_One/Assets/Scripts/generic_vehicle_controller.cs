using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generic_vehicle_controller : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    private float engineRPM;

    public void ApplyLocalPositionToVisuals(WheelCollider collider, GameObject visual)
    {
        if (visual)
        {
            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);
            visual.transform.position = position;
            visual.transform.rotation = rotation;
        }
    }

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        if (GetComponentInChildren<RealisticEngineSound>())
        {
            engineRPM = GetComponentInChildren<RealisticEngineSound>().engineCurrentRPM = 0.95f * engineRPM + 0.05f * Mathf.Abs(axleInfos[0].leftWheel.rpm * 9.5f);
        }

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel, axleInfo.leftWheelVisual);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel, axleInfo.rightWheelVisual);
        }
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public GameObject leftWheelVisual;
    public WheelCollider rightWheel;
    public GameObject rightWheelVisual;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}