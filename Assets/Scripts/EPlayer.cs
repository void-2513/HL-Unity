using System;
using System.Collections;
using UnityEngine;

public class EPlayer : MonoBehaviour
{
    public static EPlayer player;
    public GameObject hands;
    public GameObject vehiclePlayer;
    public GameObject camera;
    public PlayerAiming playerAiming;
    
    private GameObject vehiclePlayerInstance;

    private void Start()
    {
        player = this;
    }

    public void EquipSuit()
    {
        StartAdmireGlovesAnimation();
    }

    private IEnumerator ClearAdmireGloves()
    {
        yield return new WaitForSeconds(hands.GetComponent<AudioSource>().clip.length);
        
        hands.SetActive(false);
    }

    private void StartAdmireGlovesAnimation()
    {
        hands.SetActive(true);

        StartCoroutine(ClearAdmireGloves());
    }

    public void ViewPunchReset()
    {
        playerAiming.ViewPunchReset();
    }

    public void ViewPunch(Vector2 punchAmount)
    {
        playerAiming.ViewPunch(punchAmount);
    }

    public void EnterVehicle(Transform vehicleEyePosition)
    {
        vehiclePlayerInstance = Instantiate(vehiclePlayer, vehicleEyePosition.position, vehicleEyePosition.rotation, vehicleEyePosition.transform);
        camera.SetActive(false);
    }

    public void ExitVehicle(Transform exitVehiclePosition)
    {
        Destroy(vehiclePlayerInstance);
        camera.SetActive(true);
        transform.position = exitVehiclePosition.position;
    }
    
    // Helper method for weapons to easily access player
    public static EPlayer GetLocalPlayer()
    {
        if (player != null) return player;
        
        // Fallback: search by tag if singleton wasn't set
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        return playerObj ? playerObj.GetComponent<EPlayer>() : null;
    }
}
