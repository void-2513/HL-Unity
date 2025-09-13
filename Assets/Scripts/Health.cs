using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    public int maxHealth = 100;
    public bool dead = false;
    
    public virtual void TakeDamage(int damage) { }

    public virtual void Death() { }
}
