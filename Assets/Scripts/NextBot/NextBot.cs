using UnityEngine;

public class NextBot : MonoBehaviour
{
    public NextBotMovementController movementController;
    public Transform target;
    public float stoppingDistance = 2f;
    public float rotationSpeed = 5f;
    public Animator animator;
    
    private Vector3 lastTargetPos;

    void Update()
    {
        if (target == null) return;
        
        // First, rotate to face the target
        RotateTowardsTarget();
        
        // Then calculate movement
        CalculateMovement();
        
        lastTargetPos = target.position;

        if (animator != null)
        {
            animator.SetFloat("Velocity", movementController.CurrentVelocity.magnitude);
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 directionToTarget = (target.position - transform.position);
        directionToTarget.y = 0; // Ignore height
        
        if (directionToTarget.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void CalculateMovement()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        
        if (distance > stoppingDistance)
        {
            // Since we're already facing the target, we just need to move forward
            movementController.SetAIInput(0, 1f); // 0 horizontal, 1 vertical (forward)
            
            // Run if far away
            movementController.SetAIRun(distance > stoppingDistance * 3f);
            
            // Optional: Add slight horizontal movement for more natural behavior
            // movementController.SetAIInput(Random.Range(-0.1f, 0.1f), 1f);
        }
        else
        {
            // Stop when close enough
            movementController.SetAIInput(0, 0);

            if (movementController.IsGrounded)
            {
                movementController.SetAIJump(true);
            }
            else
            {
                movementController.SetAIJump(false);
            }
        }
    }

    // Visual debugging
    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        }
    }
}