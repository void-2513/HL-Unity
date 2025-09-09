using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    public static BasePlayer player;
    public GameObject hands;
    public GameObject vehiclePlayer;
    public GameObject camera;
    public GameObject weapons;
    public PlayerAiming playerAiming;
    public List<GameObject> weaponsEquiped = new List<GameObject>();
    
    private GameObject vehiclePlayerInstance;

    private void Start()
    {
        player = this;
    }

    public void EquipSuit()
    {
        StartAdmireGlovesAnimation();
    }
    
    public void EquipFirstWeapon()
    {
        if (weapons.transform.childCount <= 0)
        {
            Debug.Log("player has no weapons to disable");
            return;
        }
        
        for (int i = 0; i < weapons.transform.childCount; i++)
        {
            Debug.Log("disabling weapon " + i);
            weapons.transform.GetChild(i).gameObject.SetActive(false);
        }
        Debug.Log("enabling first weapon");
        weapons.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void DisableAllWeapons()
    {
        for (int i = 0; i < weapons.transform.childCount; i++)
        {
            weapons.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private IEnumerator ClearAdmireGloves()
    {
        yield return new WaitForSeconds(hands.GetComponent<AudioSource>().clip.length);
        
        hands.SetActive(false);
    }

    private IEnumerator AfterGlovesAnim()
    {
        yield return new WaitForSeconds(5);
        
        hands.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        
        EquipFirstWeapon();
    }

    public GameObject FindEquipWeaponIndexByName(string weapName)
    {
        for (int i = 0; i < weaponsEquiped.Count; i++)
        {
            if (weaponsEquiped[i].name == weapName)
            {
                return weaponsEquiped[i]; // Return the index if a match is found
            }
        }
        return weaponsEquiped[0]; // Return first weapon if no weapon was found
    }

    public void EquipNewWeapon(GameObject weapInstance)
    {
        DisableAllWeapons();
        Instantiate(weapInstance, weapons.transform);
        weaponsEquiped.Add(weapInstance);
        FindEquipWeaponIndexByName(weapInstance.name).SetActive(true);
    }

    private void StartAdmireGlovesAnimation()
    {
        hands.SetActive(true);

        StartCoroutine(ClearAdmireGloves());
        StartCoroutine(AfterGlovesAnim());
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
    public static BasePlayer GetLocalPlayer()
    {
        if (player != null) return player;
        
        // Fallback: search by tag if singleton wasn't set
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        return playerObj ? playerObj.GetComponent<BasePlayer>() : null;
    }

    public Transform GetAimPoint()
    {
        return camera.transform;
    }
}
