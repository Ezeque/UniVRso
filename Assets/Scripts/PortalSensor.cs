using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PortalSensor : MonoBehaviour
{
    public float detectionAngle = 30f;
    private InputDevice rightController; 
    public float vibrationAmplitude = 0.5f; 

    void Start()
    {
        rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void Update()
    {
        if (!rightController.isValid)
        {
            rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            return;
        }

        GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");

        bool portalDetected = false;

        foreach (GameObject portal in portals)
        {
            Vector3 directionToPortal = portal.transform.position - transform.position;
            directionToPortal.y = 0; 

            if (directionToPortal.magnitude == 0)
            {
                continue;
            }

            directionToPortal.Normalize();

            Vector3 forwardXZ = transform.forward;
            forwardXZ.y = 0; 
            forwardXZ.Normalize();

            float angleToPortal = Vector3.Angle(forwardXZ, directionToPortal);

            if (angleToPortal <= detectionAngle / 2f)
            {
                portalDetected = true;
                break; 
            }
        }

        if (portalDetected)
        {
            rightController.SendHapticImpulse(0, vibrationAmplitude, Time.deltaTime);
        }
        else
        {
            rightController.StopHaptics();
        }
    }
}
