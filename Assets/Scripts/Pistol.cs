using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pistol : WeaponBase
{
    public AudioClip shotSound;
    public AudioClip clickSound;
    public int damage = 5;
    public int maxClip = 18;
    public int currentClip = 18;

    private BasePlayer.AmmoType ammoType = BasePlayer.AmmoType.Pistol;
    private bool isFiring;
    private bool isEmpty;
    private bool clicked = false;
    private int numShotsFired;

    private void Awake()
    {
        SetMaxClip();
        PlayerUi.instance.SetReserveAmmo(GetReserveAmmo());
        PlayerUi.instance.SetAmmo(currentClip);
        PlayerUi.instance.EnableAmmoCount(true);
    }

    private void OnEnable()
    {
        PlayerUi.instance.EnableAmmoCount(true);
    }

    protected override void PrimaryAttack()
    {
        base.PrimaryAttack();

        if (fireRate + lastAttackTime > Time.time) return;
        
        if (currentClip == 0) return;
        
        if (isDrawing) return;

        if (BasePlayer.player.UnderwaterCheck())
        {
            if (!clicked)
            {
                weaponAudio.PlayOneShot(clickSound);
                clicked = true;
            }

            return;
        }
        
        clicked = false;
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
        PlayerUi.instance.SetAmmo(currentClip);
        PlayerUi.instance.SetReserveAmmo(BasePlayer.player.GetReserveAmmo(ammoType));

        if (Physics.Raycast(BasePlayer.player.GetAimPoint().position, BasePlayer.player.GetAimPoint().forward, out var hit, 300))
        {
            var health = hit.collider.GetComponentInParent<Health>();

            if (health)
            {
                health.TakeDamage(damage);
            }
        }

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
        
        BasePlayer player = BasePlayer.GetLocalPlayer();
        if (player == null) return;
        
        // Check if player has reserve ammo
        if (player.GetReserveAmmo(ammoType) <= 0) return;
        
        int ammoNeeded = maxClip - currentClip;
        int ammoToReload = Mathf.Min(ammoNeeded, player.GetReserveAmmo(ammoType));
        
        // Use ammo from player's reserve
        player.UseAmmo(ammoType, ammoToReload);
        currentClip += ammoToReload;
        
        animator.SetTrigger("Reload");
        
        // Reset empty state if we successfully reloaded
        if (currentClip > 0)
        {
            animator.SetBool("Empty", false);
            isEmpty = false;
        }
    }
    
    // Remove the old AddAmmo method and replace with:
    public void AddAmmoToPlayer(int amount)
    {
        BasePlayer player = BasePlayer.GetLocalPlayer();
        if (player != null)
        {
            player.AddAmmo(ammoType, amount);
        }
    }
    
    // Update getter methods to use player's reserves:
    public int GetReserveAmmo()
    {
        BasePlayer player = BasePlayer.GetLocalPlayer();
        return player != null ? player.GetReserveAmmo(ammoType) : 0;
    }
    
    public int GetMaxReserveAmmo()
    {
        BasePlayer player = BasePlayer.GetLocalPlayer();
        return player != null ? player.GetMaxReserveAmmo(ammoType) : 0;
    }

    public void SetMaxClip()
    {
        currentClip = maxClip;
        PlayerUi.instance.SetReserveAmmo(GetReserveAmmo());
        PlayerUi.instance.SetAmmo(currentClip);
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
