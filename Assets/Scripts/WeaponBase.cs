using System;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public AudioSource weaponAudio;
    public float fireRate = 0.2f;
    public Animator animator;
    
    protected float lastAttackTime;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PrimaryAttack();
        }
        
        // reload animation transitions are broken for some reason so no reloads at all until i fix it >:(
        /*
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        */
    }

    protected virtual void PrimaryAttack() { }
    
    protected virtual void Reload() { }
}
