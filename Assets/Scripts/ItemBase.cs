using System;
using Fragsurf.Movement;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public bool oneUse = true;
    
    private bool used;
    
    protected virtual void OnTouchPlayer(EPlayer player) { }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<EPlayer>();
        
        if (player != null && !used)
        {
            if (oneUse)
            {
                used = true;
            }

            OnTouchPlayer(player);
        }
    }
}