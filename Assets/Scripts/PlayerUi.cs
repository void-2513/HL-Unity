using System;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{
    public static PlayerUi instance;
    public Text hudHealthText;
    public Text hudHealthTextTitle;
    public Text hudSuitText;
    public Text hudSuitTextTitle;
    public Text hudAmmoText;
    public Text hudReserveAmmoText;
    public GameObject hudHealthCount;
    public GameObject hudSuitCount;
    public GameObject hudAmmoCount;
    
    private Color originalHealthColor;
    private Color originalHealthTitleColor;
    private Color originalSuitColor;
    private Color originalSuitTitleColor;
    private const int threshold = 45;

    private void Awake()
    {
        instance = this;
        
        // Store original colors
        if (hudHealthText != null)
            originalHealthColor = hudHealthText.color;
        
        if (hudHealthTextTitle != null)
            originalHealthTitleColor = hudHealthTextTitle.color;
        
        if (hudSuitText != null)
            originalSuitColor = hudSuitText.color;
        
        if (hudSuitTextTitle != null)
            originalSuitTitleColor = hudSuitTextTitle.color;
    }

    public void SetHealth(int health)
    {
        hudHealthText.text = health.ToString();
        UpdateHealthColor(health);
    }

    public void SetSuit(int suit)
    {
        hudSuitText.text = suit.ToString();
        UpdateSuitColor(suit);

        if (suit <= 0)
        {
            EnableSuitCount(false);
        }
    }

    public void SetAmmo(int ammo)
    {
        hudAmmoText.text = ammo.ToString();
    }

    public void SetReserveAmmo(int ammo)
    {
        hudReserveAmmoText.text = ammo.ToString();
    }
    
    public void EnableSuitCount(bool enable)
    { 
        hudSuitCount.SetActive(enable);
    }

    public void EnableAmmoCount(bool enable)
    {
        hudAmmoCount.SetActive(enable);
    }

    public void EnableHud(bool enable)
    {
        hudHealthCount.SetActive(enable);
        hudSuitCount.SetActive(enable);
        // hudAmmoCount.SetActive(enable);
    }

    public void EnableHealthCount(bool enable)
    {
        hudHealthCount.gameObject.SetActive(enable);
    }
    
    private void UpdateHealthColor(int health)
    {
        if (hudHealthText == null || hudHealthTextTitle == null) return;
        
        if (health < threshold)
        {
            // Calculate green value based on health percentage below threshold
            float healthPercentage = Mathf.Clamp01((float)health / threshold);
            
            // Update health value text color
            Color newHealthColor = new Color(originalHealthColor.r, originalHealthColor.g * healthPercentage, originalHealthColor.b);
            hudHealthText.color = newHealthColor;
            
            // Update health title text color (same calculation)
            Color newTitleColor = new Color(originalHealthTitleColor.r, originalHealthTitleColor.g * healthPercentage, originalHealthTitleColor.b);
            hudHealthTextTitle.color = newTitleColor;
        }
        else
        {
            // Reset to original colors if above threshold
            hudHealthText.color = originalHealthColor;
            hudHealthTextTitle.color = originalHealthTitleColor;
        }
    }
    
    private void UpdateSuitColor(int suit)
    {
        if (hudSuitText == null || hudSuitTextTitle == null) return;
        
        if (suit < threshold)
        {
            // Calculate green value based on suit percentage below threshold
            float suitPercentage = Mathf.Clamp01((float)suit / threshold);
            
            // Update suit value text color
            Color newSuitColor = new Color(originalSuitColor.r, originalSuitColor.g * suitPercentage, originalSuitColor.b);
            hudSuitText.color = newSuitColor;
            
            // Update suit title text color (same calculation)
            Color newTitleColor = new Color(originalSuitTitleColor.r, originalSuitTitleColor.g * suitPercentage, originalSuitTitleColor.b);
            hudSuitTextTitle.color = newTitleColor;
        }
        else
        {
            // Reset to original colors if above threshold
            hudSuitText.color = originalSuitColor;
            hudSuitTextTitle.color = originalSuitTitleColor;
        }
    }
}