using UnityEngine;

public class Crowbar : WeaponBase
{
    public AudioClip shotSound;
    public int damage = 10;

    private bool isFiring;
    private bool isEmpty;
    private int numShotsFired;

    private void Awake()
    {

    }

    protected override void PrimaryAttack()
    {
        base.PrimaryAttack();

        if (fireRate + lastAttackTime > Time.time) return;
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

        if (Physics.Raycast(BasePlayer.player.GetAimPoint().position, BasePlayer.player.GetAimPoint().forward, out var hit, 1.5f))
        {
            animator.SetTrigger("HitKill");
        }
        else
        {
            animator.SetTrigger("MissKill");
        }
        
        lastAttackTime = Time.time;
        // weaponAudio.PlayOneShot(shotSound);
        
        BasePlayer player = BasePlayer.GetLocalPlayer();
        if ( player )
        {
            player.ViewPunchReset();
        }

        AddViewKick();
    }

    private void AddViewKick()
    {
        BasePlayer player = BasePlayer.GetLocalPlayer();
        
        Vector2 punchAng;
        punchAng.x = Random.Range( 1.0f, 2.0f );
        punchAng.y = Random.Range( -2.0f, -1.0f );

        //Add it to the view punch
        player.ViewPunch( punchAng );
    }

    protected override void Reload()
    {
        base.Reload();

        return;
    }

    public void FinishDraw()
    {
        isDrawing = false;
    }
}
