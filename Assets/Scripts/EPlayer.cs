using UnityEngine;

public class EPlayer : MonoBehaviour
{
    public GameObject hands;
    
    public void EquipSuit()
    {
        StartAdmireGlovesAnimation();
    }

    private void StartAdmireGlovesAnimation()
    {
        hands.SetActive(true);
    }
}
