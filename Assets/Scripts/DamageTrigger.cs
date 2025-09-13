using System;
using UnityEngine;

public class DamageTrigger : Entity
{
    public int damage = 20;

    public override void OnInteract(GameObject interactor)
    {
        base.OnInteract(interactor);

        interactor.GetComponentInParent<Health>().TakeDamage(damage);
    }
}
