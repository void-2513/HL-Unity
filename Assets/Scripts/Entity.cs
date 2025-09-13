using UnityEngine;

public class Entity : MonoBehaviour
{
    public BasePlayer owner;
    
    public virtual void OnInteract(GameObject interactor) { }
    public virtual void OnHoldInteract(GameObject interactor) { }

    public void AssignOwner(BasePlayer player)
    {
        owner = player;
    }

    public void RemoveOwner()
    {
        owner = null;
    }
}
