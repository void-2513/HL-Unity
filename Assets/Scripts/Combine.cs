using UnityEngine;
using UnityEngine.AI;

public class Combine : Health
{
    public Transform target;
    public Transform shootingPoint;
    public float fireRate = 1.2f;
    public Ragdoll ragdoll;
    public GameObject heldWeapon;
    public AudioSource audio;
    public AudioClip fireSound;
    public AudioClip pumpSound;
    
    private NavMeshAgent agent;
    private Animator animator;
    private float lastFireTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Shoot();
        }
        
        animator.SetFloat("vely", agent.velocity.magnitude);
    }

    void Shoot()
    {
        if (fireRate + lastFireTime > Time.time) return;
        
        lastFireTime = Time.time;
        
        animator.SetTrigger("fire");
        
        audio.PlayOneShot(fireSound);

        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out var hit, 120f))
        {
            hit.collider.GetComponentInParent<Health>()?.TakeDamage(30);
        }
    }

    public void PlayPumpSound()
    {
        audio.PlayOneShot(pumpSound);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        
        health -= damage;

        if (health <= 0)
        {
            Death();
        }
    }

    public override void Death()
    {
        base.Death();
        
        ragdoll.SetRagdoll(true);
        DropWeapon();
        enabled = false;
    }

    // Optional: Visual debugging
    void OnDrawGizmos()
    {
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, agent.destination);
            
            // Draw desired velocity
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, agent.desiredVelocity.normalized * 2f);
        }
    }

    void DropWeapon()
    {
        dead = true;
        heldWeapon.transform.parent = null;
        heldWeapon.GetComponent<Rigidbody>().isKinematic = false;
    }
}