using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    // This component is separate from the player's underwater movement.
    // Feel free to add whatever you want in here, like a rigidbody buoyancy/floating system or something.

    public AudioClip splash;
    public GameObject splashEffect;
    
    [Header("Buoyancy Settings")]
    public float buoyancyForce = 9.8f;
    public float waterDrag = 1f;
    public float waterAngularDrag = 1f;
    public float waterLevelOffset = 0f;
    
    [Header("Wave Settings (Optional)")]
    public float waveFrequency = 1f;
    public float waveAmplitude = 0.1f;
    
    // Track objects in water
    private Dictionary<Rigidbody, WaterObjectData> objectsInWater = new Dictionary<Rigidbody, WaterObjectData>();
    
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !objectsInWater.ContainsKey(rb))
        {
            Vector3 relativeVelocity = rb.linearVelocity;
            float impactSpeed = relativeVelocity.magnitude;
        
            // Check if impact is strong enough for splash
            if (impactSpeed >= 7f)
            {
                Instantiate(splashEffect, rb.position, Quaternion.identity);
                AudioSource.PlayClipAtPoint(splash, other.transform.position);
            }
            
            // Store original drag values
            objectsInWater[rb] = new WaterObjectData
            {
                originalDrag = rb.linearDamping,
                originalAngularDrag = rb.angularDamping
            };
            
            // Apply water drag
            rb.linearDamping = waterDrag;
            rb.angularDamping = waterAngularDrag;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && objectsInWater.ContainsKey(rb))
        {
            // Restore original drag values
            WaterObjectData data = objectsInWater[rb];
            rb.linearDamping = data.originalDrag;
            rb.angularDamping = data.originalAngularDrag;
            
            objectsInWater.Remove(rb);
        }
    }
    
    private void FixedUpdate()
    {
        foreach (var kvp in objectsInWater)
        {
            Rigidbody rb = kvp.Key;
            if (rb != null)
            {
                ApplyBuoyancy(rb);
            }
        }
    }
    
    private void ApplyBuoyancy(Rigidbody rb)
    {
        // Calculate water level at object's position (with optional waves)
        float waterLevel = CalculateWaterLevel(rb.position);
        
        // Calculate how submerged the object is
        float submergence = CalculateSubmergence(rb, waterLevel);
        
        if (submergence > 0)
        {
            // Apply buoyancy force (Archimedes' principle)
            Vector3 buoyantForce = Vector3.up * buoyancyForce * submergence * rb.mass;
            rb.AddForce(buoyantForce, ForceMode.Force);
            
            // Optional: Add damping force for more realistic water resistance
            Vector3 velocityDamping = -rb.linearVelocity * waterDrag * submergence;
            rb.AddForce(velocityDamping, ForceMode.Force);
        }
    }
    
    private float CalculateWaterLevel(Vector3 position)
    {
        float baseWaterLevel = transform.position.y + waterLevelOffset;
        
        // Add simple wave effect if enabled
        if (waveAmplitude > 0 && waveFrequency > 0)
        {
            float wave = Mathf.Sin(Time.time * waveFrequency + position.x * 0.5f + position.z * 0.5f) * waveAmplitude;
            return baseWaterLevel + wave;
        }
        
        return baseWaterLevel;
    }
    
    private float CalculateSubmergence(Rigidbody rb, float waterLevel)
    {
        // Simple approximation: use the bottom of the collider bounds
        // For more accuracy, you might want to sample multiple points
        float objectBottom = rb.GetComponent<Collider>().bounds.min.y;
        float objectHeight = rb.GetComponent<Collider>().bounds.size.y;
        
        // Calculate how much of the object is submerged (0-1)
        float depth = waterLevel - objectBottom;
        float submergence = Mathf.Clamp01(depth / objectHeight);
        
        return submergence;
    }
    
    // Helper class to store object data
    private class WaterObjectData
    {
        public float originalDrag;
        public float originalAngularDrag;
    }
    
    // Optional: Draw water level gizmo in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        float waterLevel = transform.position.y + waterLevelOffset;
        Vector3 center = new Vector3(transform.position.x, waterLevel, transform.position.z);
        Vector3 size = new Vector3(transform.localScale.x * 10f, 0.1f, transform.localScale.z * 10f);
        Gizmos.DrawWireCube(center, size);
    }
}
