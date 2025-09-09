using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Jeep : Entity
{
    public Transform viewPoint;
    public Transform exitPosition;
    public AudioSource engineRumbleAudio;
    public AudioClip engineStartClip;
    public AudioClip engineStopClip;
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public float brakeTorque = 500f; // braking force
    public float brakeInputThreshold = 0.1f; // minimum input to consider as braking
    public Rigidbody rigidbody;
    public Transform steeringwheel;

    private bool hasDriver;
    private BasePlayer driver;
    private Quaternion initSteerRot;

    private void Start()
    {
       
    }

    public override void OnInteract(GameObject interactor)
    {
        base.OnInteract(interactor);

        if (!hasDriver)
        {
            Activate();
            driver = interactor.GetComponentInParent<BasePlayer>();
            AssignOwner(driver);
            driver.EnterVehicle(viewPoint);
            
            hasDriver = true;
        }
    }

    private void Update()
    {
        if (hasDriver)
        {
            if (Input.GetKeyDown(KeyCode.E) && CanExitVehicle())
            {
                driver.ExitVehicle(exitPosition);
                RemoveOwner();
                StartCoroutine(StopEngineRumble());
                driver = null;
                hasDriver = false;
            }
        }
    }
    
    public void ApplyLocalPositionToVisuals(WheelCollider collider, Transform visualWheel)
    {
        if (visualWheel == null) {
            return;
        }
     
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
     
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    private void RotateSteeringWheel()
    {
        if (!steeringwheel) return;

        float steeringInput = Input.GetAxis("Horizontal");
        float targetAngle = maxSteeringAngle * steeringInput;
    
        // Create a local rotation around the Z axis
        Quaternion targetLocalRotation = Quaternion.Euler(0f, targetAngle, 0f);
    
        // Apply the local rotation
        steeringwheel.localRotation = targetLocalRotation;
    }

    private void FixedUpdate()
    {
        if (hasDriver)
        {
            float motor = maxMotorTorque * Input.GetAxis("Vertical");
            float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
            float brake = 0f;
        
            // Apply brakes when no vertical input is detected
            if (Mathf.Abs(Input.GetAxis("Vertical")) < 0.1f)
            {
                brake = brakeTorque;
                motor = 0f;
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
            
                axleInfo.leftWheel.brakeTorque = brake;
                axleInfo.rightWheel.brakeTorque = brake;
            
                ApplyLocalPositionToVisuals(axleInfo.leftWheel, axleInfo.visL);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel, axleInfo.visR);
                RotateSteeringWheel();
            }
        }
    }

    public void Activate()
    {
        StartCoroutine(StartEngineRumble());
    }

    private IEnumerator StartEngineRumble()
    {
        engineRumbleAudio.PlayOneShot(engineStartClip);
        
        yield return new WaitForSeconds(engineRumbleAudio.clip.length);
        
        engineRumbleAudio.Play();
    }

    private IEnumerator StopEngineRumble()
    {
        engineRumbleAudio.PlayOneShot(engineStopClip);
        
        yield return new WaitForSeconds(engineRumbleAudio.clip.length);
        
        engineRumbleAudio.Stop();
    }

    private bool CanExitVehicle()
    {
        return rigidbody.linearVelocity.magnitude < 0.1f;
    }
}

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public Transform visL;
    public Transform visR;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}