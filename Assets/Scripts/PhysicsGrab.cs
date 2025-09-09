using System;
using UnityEngine;

public class PhysicsGrab : MonoBehaviour
{
    public Transform holdPoint;
    public GameObject weapons;

    private bool isHolding;
    private Rigidbody heldRigidbody;
    
    private void Update()
    {
        weapons.SetActive(!isHolding);
        
        if (isHolding && heldRigidbody)
        {
            SetObjectPosition();
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHolding)
            {
                if (Physics.Raycast(transform.position, transform.forward, out var hit, 3))
                {
                    var rigidbody = hit.collider.gameObject.GetComponentInParent<Rigidbody>();

                    if (rigidbody && rigidbody.mass < 60)
                    {
                        GrabObject(rigidbody);
                    }
                }
            }
            else
            {
                DropObject();
            }
        }

        if (Input.GetMouseButtonDown(0) && isHolding && heldRigidbody)
        {
            ThrowObject();
        }
    }

    private void GrabObject(Rigidbody rigidbody)
    {
        isHolding = true;
                    
        heldRigidbody = rigidbody;
        heldRigidbody.isKinematic = true;
        heldRigidbody.useGravity = false;
    }

    private void DropObject()
    {
        isHolding = false;
                
        heldRigidbody.isKinematic = false;
        heldRigidbody.useGravity = true;
        heldRigidbody = null;
    }

    private void ThrowObject()
    {
        isHolding = false;
                
        heldRigidbody.isKinematic = false;
        heldRigidbody.useGravity = true;
        heldRigidbody.AddForce(20 * transform.forward, ForceMode.Impulse);
        heldRigidbody = null;
    }

    private void SetObjectPosition()
    {
        heldRigidbody.MovePosition(holdPoint.position);
        heldRigidbody.MoveRotation(holdPoint.rotation);
    }
}