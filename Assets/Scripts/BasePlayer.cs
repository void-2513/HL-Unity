using System;
using System.Collections;
using System.Collections.Generic;
using Fragsurf.Movement;
using UnityEngine;
using UnityEngine.Audio;

public class BasePlayer : Health
{
    public static BasePlayer player;
    public Vector3 deadCamOffset;
    public GameObject hands;
    public GameObject vehiclePlayer;
    public GameObject camera;
    public GameObject weapons;
    public GameObject overlayWater;
    public PlayerAiming playerAiming;
    public List<GameObject> weaponsEquiped = new List<GameObject>();
    public SurfCharacter surfCharacter;
    public AudioMixer audioMixer;
    public int primaryAmmo = 100;
    public int secondaryAmmo = 100;
    
    // Weapon switching properties
    private int currentWeaponIndex = 0;
    private bool isSwitchingWeapons = false;
    public float weaponSwitchTime = 0.5f;
    public AudioClip weaponSwitchSound;
    
    // Suit properties
    public bool hasSuit = false;
    public int suitHealth = 0;
    public int maxSuitHealth = 100;
    public float suitDamageAbsorption = 0.6f; // 60% damage absorbed by suit
    
    // Ammo types
    public enum AmmoType
    {
        Pistol,
        Shotgun,
        SMG,
        Rifle,
        Explosive
    }
    
    // Ammo reserves dictionary
    private Dictionary<AmmoType, int> ammoReserves = new Dictionary<AmmoType, int>();
    private Dictionary<AmmoType, int> maxAmmoReserves = new Dictionary<AmmoType, int>();
    private GameObject vehiclePlayerInstance;

    private void Start()
    {
        player = this;
        
        health = maxHealth;
        suitHealth = 20;
        
        // Initialize ammo reserves
        InitializeAmmoReserves();
        
        // Initialize weapon system
        InitializeWeapons();
    }

    private void InitializeWeapons()
    {
        if (weapons.transform.childCount > 0)
        {
            // Disable all weapons initially
            DisableAllWeapons();
            
            // Enable first weapon if available
            if (weapons.transform.childCount > 0)
            {
                weapons.transform.GetChild(0).gameObject.SetActive(true);
                currentWeaponIndex = 0;
            }
        }
    }

    private void InitializeAmmoReserves()
    {
        // Set up default ammo values
        ammoReserves[AmmoType.Pistol] = 100;
        ammoReserves[AmmoType.Shotgun] = 50;
        ammoReserves[AmmoType.SMG] = 200;
        ammoReserves[AmmoType.Rifle] = 150;
        ammoReserves[AmmoType.Explosive] = 10;
        
        // Set up maximum ammo capacities
        maxAmmoReserves[AmmoType.Pistol] = 200;
        maxAmmoReserves[AmmoType.Shotgun] = 100;
        maxAmmoReserves[AmmoType.SMG] = 400;
        maxAmmoReserves[AmmoType.Rifle] = 300;
        maxAmmoReserves[AmmoType.Explosive] = 20;
    }

    private void Update()
    {
        overlayWater.SetActive(UnderwaterCheck());

        if (UnderwaterCheck())
        {
            audioMixer.SetFloat("MyExposedParam", 397);
            audioMixer.SetFloat("MyExposedParam2", 2);
        }
        else
        {
            audioMixer.SetFloat("MyExposedParam", 22000);
            audioMixer.SetFloat("MyExposedParam2", 1);
        }
        
        if (health <= 0 && !dead)
        {
            Death();
            dead = true;
        }
        
        // Handle weapon switching input
        HandleWeaponSwitchingInput();
    }

    private void HandleWeaponSwitchingInput()
    {
        if (isSwitchingWeapons || weapons.transform.childCount <= 1) return;
        
        // Mouse wheel switching
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
            {
                SwitchToNextWeapon();
            }
            else
            {
                SwitchToPreviousWeapon();
            }
        }
        
        // Number key switching
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchToWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchToWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SwitchToWeapon(4);
    }

    public void SwitchToNextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % weapons.transform.childCount;
        StartCoroutine(SwitchWeapon(nextIndex));
    }

    public void SwitchToPreviousWeapon()
    {
        int previousIndex = (currentWeaponIndex - 1 + weapons.transform.childCount) % weapons.transform.childCount;
        StartCoroutine(SwitchWeapon(previousIndex));
    }

    public void SwitchToWeapon(int newIndex)
    {
        if (newIndex >= 0 && newIndex < weapons.transform.childCount && newIndex != currentWeaponIndex)
        {
            StartCoroutine(SwitchWeapon(newIndex));
        }
    }

    private IEnumerator SwitchWeapon(int newIndex)
    {
        if (isSwitchingWeapons || newIndex == currentWeaponIndex) yield break;
        
        isSwitchingWeapons = true;
        
        // Play switch sound
        if (weaponSwitchSound != null)
        {
            AudioSource.PlayClipAtPoint(weaponSwitchSound, transform.position);
        }
        
        // Disable current weapon
        weapons.transform.GetChild(currentWeaponIndex).gameObject.SetActive(false);
        
        // Wait for switch time
        yield return new WaitForSeconds(weaponSwitchTime);
        
        // Enable new weapon
        weapons.transform.GetChild(newIndex).gameObject.SetActive(true);
        currentWeaponIndex = newIndex;
        
        isSwitchingWeapons = false;
        
        // Update UI if needed
        UpdateWeaponUI();
    }

    public GameObject GetCurrentWeapon()
    {
        if (weapons.transform.childCount == 0) return null;
        return weapons.transform.GetChild(currentWeaponIndex).gameObject;
    }

    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }

    public int GetWeaponCount()
    {
        return weapons.transform.childCount;
    }

    public bool IsSwitchingWeapons()
    {
        return isSwitchingWeapons;
    }

    private void UpdateWeaponUI()
    {
        // Update UI with current weapon information
        // Example: PlayerUi.instance.UpdateWeaponDisplay(currentWeaponIndex, GetCurrentWeapon().name);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        
        int finalDamage = CalculateDamageWithSuit(damage);
        health -= finalDamage;
        PlayerUi.instance.SetHealth(health);
        
        // Update UI for suit health if needed
        if (hasSuit)
        {
            PlayerUi.instance.SetSuit(suitHealth);
        }
    }

    private int CalculateDamageWithSuit(int incomingDamage)
    {
        if (!hasSuit || suitHealth <= 0)
        {
            return incomingDamage; // No suit protection
        }

        // Calculate damage absorbed by suit
        int suitAbsorbedDamage = Mathf.RoundToInt(incomingDamage * suitDamageAbsorption);
        int playerDamage = incomingDamage - suitAbsorbedDamage;
        
        // Apply damage to suit first
        int actualSuitDamage = Mathf.Min(suitAbsorbedDamage, suitHealth);
        suitHealth -= actualSuitDamage;
        
        // If suit couldn't absorb all the damage it was supposed to,
        // the remaining damage goes to the player
        int remainingDamage = suitAbsorbedDamage - actualSuitDamage;
        playerDamage += remainingDamage;
        
        Debug.Log($"Damage Calculation: {incomingDamage} total -> " +
                  $"{actualSuitDamage} to suit -> {playerDamage} to player | " +
                  $"Suit health: {suitHealth}");
        
        return playerDamage;
    }

    public void RepairSuit(int repairAmount)
    {
        if (!hasSuit) return;
        
        suitHealth = Mathf.Min(suitHealth + repairAmount, maxSuitHealth);
        PlayerUi.instance.SetSuit(suitHealth);
        Debug.Log($"Suit repaired: {repairAmount}. Current suit health: {suitHealth}");
    }

    public override void Death()
    {
        base.Death();
        
        camera.transform.Translate(deadCamOffset);
        surfCharacter.enabled = false;
        
        // Disable weapons on death
        DisableAllWeapons();
    }

    public void EquipSuit()
    {
        StartAdmireGlovesAnimation();
        
        PlayerUi.instance.EnableHud(true);
        PlayerUi.instance.SetSuit(suitHealth);
        PlayerUi.instance.SetHealth(health);
        hasSuit = true;
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
        currentWeaponIndex = 0;
        
        UpdateWeaponUI();
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
        // Check if weapon already exists
        for (int i = 0; i < weapons.transform.childCount; i++)
        {
            if (weapons.transform.GetChild(i).name == weapInstance.name)
            {
                // Weapon already equipped, just switch to it
                SwitchToWeapon(i);
                return;
            }
        }
        
        // Add new weapon
        GameObject newWeapon = Instantiate(weapInstance, weapons.transform);
        weaponsEquiped.Add(weapInstance);
        
        // Disable all weapons first
        DisableAllWeapons();
        
        // Enable the new weapon
        newWeapon.SetActive(true);
        currentWeaponIndex = weapons.transform.childCount - 1;
        
        UpdateWeaponUI();
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
        
        // Hide weapons when entering vehicle
        DisableAllWeapons();
    }

    public void ExitVehicle(Transform exitVehiclePosition)
    {
        Destroy(vehiclePlayerInstance);
        camera.SetActive(true);
        transform.position = exitVehiclePosition.position;
        
        // Show weapons again when exiting vehicle
        if (weapons.transform.childCount > 0)
        {
            weapons.transform.GetChild(currentWeaponIndex).gameObject.SetActive(true);
        }
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

    public bool UnderwaterCheck()
    {
        var wc = GetComponent<SurfCharacter>()._cameraWaterCheckObject.GetComponent<CameraWaterCheck>();
        
        return wc.IsUnderwater();
    }
    
    // New method to check if player has functional suit
    public bool HasFunctionalSuit()
    {
        return hasSuit && suitHealth > 0;
    }

    // New method to get suit protection percentage (for UI or other systems)
    public float GetSuitProtectionPercentage()
    {
        if (!hasSuit) return 0f;
        return (float)suitHealth / maxSuitHealth * suitDamageAbsorption;
    }
    
    // ========== AMMO RESERVE METHODS ==========
    
    // Get current reserve ammo for a specific type
    public int GetReserveAmmo(AmmoType ammoType)
    {
        if (ammoReserves.ContainsKey(ammoType))
        {
            return ammoReserves[ammoType];
        }
        return 0;
    }
    
    // Get maximum reserve ammo for a specific type
    public int GetMaxReserveAmmo(AmmoType ammoType)
    {
        if (maxAmmoReserves.ContainsKey(ammoType))
        {
            return maxAmmoReserves[ammoType];
        }
        return 0;
    }
    
    // Add ammo to reserve (returns actual amount added)
    public int AddAmmo(AmmoType ammoType, int amount)
    {
        if (!ammoReserves.ContainsKey(ammoType))
        {
            ammoReserves[ammoType] = 0;
        }
        
        int currentAmmo = ammoReserves[ammoType];
        int maxAmmo = GetMaxReserveAmmo(ammoType);
        int spaceAvailable = maxAmmo - currentAmmo;
        int amountToAdd = Mathf.Min(amount, spaceAvailable);
        
        ammoReserves[ammoType] += amountToAdd;
        
        // Update UI if needed
        UpdateAmmoUI();
        
        return amountToAdd;
    }
    
    // Use ammo from reserve (returns actual amount used)
    public int UseAmmo(AmmoType ammoType, int amount)
    {
        if (!ammoReserves.ContainsKey(ammoType))
        {
            return 0;
        }
        
        int currentAmmo = ammoReserves[ammoType];
        int amountToUse = Mathf.Min(amount, currentAmmo);
        
        ammoReserves[ammoType] -= amountToUse;
        
        // Update UI if needed
        UpdateAmmoUI();
        
        return amountToUse;
    }
    
    // Check if player has enough ammo of a specific type
    public bool HasAmmo(AmmoType ammoType, int amount)
    {
        return GetReserveAmmo(ammoType) >= amount;
    }
    
    // Update UI with current ammo counts
    private void UpdateAmmoUI()
    {
        // You can implement UI updates here
        // For example: PlayerUi.instance.UpdateAmmoDisplay(ammoReserves);
    }
    
    // Set ammo directly (for cheats or initialization)
    public void SetAmmo(AmmoType ammoType, int amount)
    {
        int maxAmmo = GetMaxReserveAmmo(ammoType);
        ammoReserves[ammoType] = Mathf.Clamp(amount, 0, maxAmmo);
        UpdateAmmoUI();
    }
}