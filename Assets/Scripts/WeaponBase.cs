using System;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public AudioSource weaponAudio;
    public float fireRate = 0.2f;
    public Animator animator;
    
    protected float lastAttackTime;
    protected bool isDrawing = true;

    private void Start()
    {
        weaponAudio = GetComponentInParent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            PrimaryAttack();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    protected virtual void PrimaryAttack() { }
    
    protected virtual void Reload() { }
}
