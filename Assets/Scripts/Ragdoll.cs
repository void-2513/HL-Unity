using System;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Rigidbody[] limbs;
    public Animator animator;

    private void Awake() => limbs = GetComponentsInChildren<Rigidbody>();

    public void SetRagdoll(bool state)
    {
        animator.enabled = !state;
        
        foreach (var limb in limbs)
        {
            limb.isKinematic = !state;
        }
    }
}
