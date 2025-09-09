using System.Linq.Expressions;
using UnityEngine;

public class PhysicsGib : MonoBehaviour
{
    public GameObject normmodel;
    public GameObject gibmodel;
    
    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        { 
            Debug.DrawRay(contact.point, contact.normal, Color.white); 
        }

        if (collision.relativeVelocity.magnitude > 2)
        {
            normmodel.SetActive(false);
            gibmodel.SetActive(true);
        }
    }
}
