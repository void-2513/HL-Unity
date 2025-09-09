using UnityEngine;

public class Item_HEV : ItemBase
{
    public GameObject model;
    
    protected override void OnTouchPlayer(BasePlayer player)
    {
        base.OnTouchPlayer(player);
        
        player.EquipSuit();
        model.SetActive(false);
    }
}
