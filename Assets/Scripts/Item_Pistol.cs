using UnityEngine;

public class Item_Pistol : ItemBase
{
    public GameObject model;
    public GameObject prefab;
    
    protected override void OnTouchPlayer(BasePlayer player)
    {
        base.OnTouchPlayer(player);
        
        player.EquipNewWeapon(prefab);
        model.SetActive(false);
    }
}
