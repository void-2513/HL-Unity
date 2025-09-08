using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pistol : WeaponBase
{
    public AudioClip shotSound;
    public int damage = 5;
    public int maxClip = 18;
    public int currentClip = 18;

    private bool isFiring;

    private void Awake()
    {
        SetMaxClip();
    }

    protected override void PrimaryAttack()
    {
        base.PrimaryAttack();

        if (fireRate + lastAttackTime > Time.time) return;
        
        if (currentClip == 0) return;

        isFiring = true;
        
        lastAttackTime = Time.time;
        animator.SetTrigger("Fire");
        weaponAudio.PlayOneShot(shotSound);
        
        EPlayer player = EPlayer.GetLocalPlayer();
        if (player)
        {
            player.ViewPunchReset();
        }

        currentClip--;

        AddViewKick();
    }

    private void AddViewKick()
    {
        EPlayer player = EPlayer.GetLocalPlayer();
        
        Vector2 viewPunch;
        viewPunch.x = Random.Range( 0.25f, 0.5f );
        viewPunch.y = Random.Range( -.6f, .6f );

        //Add it to the view punch
        player.ViewPunch( viewPunch );
    }

    protected override void Reload()
    {
        base.Reload();
        
        animator.SetTrigger("Reload");
    }

    public void SetMaxClip()
    {
        currentClip = maxClip;
    }

    public void SetFireNo()
    {
        isFiring = false;
    }
}
