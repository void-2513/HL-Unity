using UnityEngine;

public class CombineAnimator : MonoBehaviour
{
    private Animator animator;
    
    // Animation layers
    private int baseLayer = 0;
    private int upperBodyLayer = 1;
    private int gestureLayer = 2;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        
        // Set layer weights
        animator.SetLayerWeight(baseLayer, 1.0f);
        animator.SetLayerWeight(upperBodyLayer, 0.8f); // Upper body aiming
        animator.SetLayerWeight(gestureLayer, 1.0f);   // Gestures/attacks
    }
    
    public void SetMovement(float speed, float direction)
    {
        animator.SetFloat("Speed", speed);
        animator.SetFloat("Direction", direction);
    }
    
    public void PlayUpperBodyAction(string stateName, float transitionTime = 0.1f)
    {
        animator.CrossFadeInFixedTime(stateName, transitionTime, upperBodyLayer);
    }
    
    public void PlayGesture(string stateName, float transitionTime = 0.1f)
    {
        animator.CrossFadeInFixedTime(stateName, transitionTime, gestureLayer);
    }
}