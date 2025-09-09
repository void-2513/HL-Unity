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
    private bool isEmpty;
    private int numShotsFired;

    private void Awake()
    {
        SetMaxClip();
    }

    protected override void PrimaryAttack()
    {
        base.PrimaryAttack();

        if (fireRate + lastAttackTime > Time.time) return;
        
        if (currentClip == 0) return;
        
        if (isDrawing) return;

        isFiring = true;

        if ( ( Time.time - lastAttackTime ) < 0.5f )
        {
            numShotsFired = 0;
        }
        else
        {
            numShotsFired++;
        }

        if ((currentClip - 1) == 0)
        {
            animator.SetTrigger("LastFire");
            animator.SetBool("Empty", true);
            isEmpty = true;
        }
        else
        {
            animator.SetTrigger("Fire");
        }
        
        lastAttackTime = Time.time;
        weaponAudio.PlayOneShot(shotSound);
        
        BasePlayer player = BasePlayer.GetLocalPlayer();
        if ( player )
        {
            player.ViewPunchReset();
        }

        currentClip--;

        AddViewKick();
    }

    private void AddViewKick()
    {
        BasePlayer player = BasePlayer.GetLocalPlayer();
        
        Vector2 viewPunch;
        viewPunch.x = Random.Range( 0.25f, 0.5f );
        viewPunch.y = Random.Range( -.6f, .6f );

        //Add it to the view punch
        player.ViewPunch( viewPunch );
    }

    protected override void Reload()
    {
        base.Reload();
        
        if (currentClip == maxClip || isDrawing) return;
        
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

    public void FinishDraw()
    {
        isDrawing = false;
    }
}
