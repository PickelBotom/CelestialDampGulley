using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private Item trashItem;
    [SerializeField] private GameObject pickUpItemPrefab;
    [SerializeField] private Vector3[] trashSpawnPoints;
    [SerializeField] private Transform trashParent;
    [SerializeField] private Image environmentTint;
    [SerializeField] private Volume lightVolume;

    private const int trashSpawnCount = 3;
    public float environmentalHealth { get; private set; } = 100f;
    private const int minHealth = 0;
    private const int maxHealth = 100; 
    private const float maxTintAlpha = 0.5f;
    private float airQuality = 100f;
    private float soilHealth = 100f;

    public float AirQuality => airQuality;
    public float SoilHealth => soilHealth;
    public int TrashCount => trashParent.childCount;
    private ColorAdjustments colorAdjustments;

    public Action<int> onEnvironmentalHealthChange;

    private void Start()
    {
        // Access the Color Adjustments effect directly from the Volume component
        if (lightVolume.sharedProfile != null && lightVolume.sharedProfile.TryGet(out colorAdjustments))
        {
            // Successfully retrieved color adjustments settings
        }
        else
        {
            Debug.LogWarning("Color Adjustments not found in the Light2D Volume.");
        }

        // Set initial environmental health based on the starting trash count
        CalculateEnvironmentHealth();

        // Spawn trash at the start of the game
        SpawnTrash(trashSpawnCount);
    }

    private void SpawnTrash(int amount)
    {
        int spawnCount = Mathf.Min(amount, trashSpawnPoints.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = trashSpawnPoints[i];
            GameObject trashObject = Instantiate(GameManager.instance.PickUpItemPrefab, spawnPosition, Quaternion.identity, trashParent);

            PickUpItem pickUpItem = trashObject.GetComponent<PickUpItem>();
            if (pickUpItem != null)
            {
                pickUpItem.Set(trashItem, 1);
            }
        }

        CalculateEnvironmentHealth(); // Update health after spawning trash
    }

    private void CalculateEnvironmentHealth()
    {
        // Set environmental health based on the amount of trash
        int trashCount = TrashCount;
        environmentalHealth = Mathf.Clamp(100 - trashCount * 5, minHealth, maxHealth);

        // Notify any listeners of the updated health value
        onEnvironmentalHealthChange?.Invoke((int)environmentalHealth);

        Debug.Log($"Environmental health recalculated based on trash count: {environmentalHealth}");

        // Update saturation and environment tint based on new health value
        UpdateSaturation();
        UpdateEnvironmentTint();
    }

    public void PickUpTrash()
{
    // Call this only if there is trash to pick up
    if (TrashCount > 0)
    {
        // Log trash picking action
        Debug.Log("Trash picked up.");
        
        // Assume there's logic here to actually remove a trash item

        CalculateEnvironmentHealth(); // Recalculate health when trash is picked up
        UpdateAirQuality(); // Update air quality after picking up trash
    }
}

    public void UseFertilizer(bool isGood)
    {
        if (isGood)
        {
            ModifyEnvironmentalHealth(15);
        }
        else
        {
            ModifyEnvironmentalHealth(-15);
        }
    }

    private void ModifyEnvironmentalHealth(int amount)
    {
        environmentalHealth = Mathf.Clamp(environmentalHealth + amount, minHealth, maxHealth);
        onEnvironmentalHealthChange?.Invoke((int)environmentalHealth);

        Debug.Log($"Environmental health changed by {amount}. Current health: {environmentalHealth}");

        // Adjust saturation based on updated health
        UpdateSaturation();
        UpdateEnvironmentTint();
    }

    private void UpdateSaturation()
    {
        if (colorAdjustments != null)
        {
            float saturation = Mathf.Lerp(-100f, 0f, environmentalHealth / maxHealth);
            colorAdjustments.saturation.Override(saturation);
        }
    }

    private void UpdateEnvironmentTint()
    {
        if (environmentTint != null)
        {
            float tintAlpha = environmentalHealth < 80 ? Mathf.Lerp(maxTintAlpha, 0, environmentalHealth / 80f) : 0f;
            Color tint = environmentTint.color;
            tint.a = tintAlpha;
            environmentTint.color = tint;
        }
    }

    public void UpdateAirQuality()
    {
        if (TrashCount > 0)
        {
            airQuality -= 0.1f * TrashCount;
            airQuality = Mathf.Clamp(airQuality, 0, 100);
        }
    }

    public void UpdateSoilHealth(bool usedSyntheticFertilizer)
    {
        soilHealth += usedSyntheticFertilizer ? -2f : 1f;
        soilHealth = Mathf.Clamp(soilHealth, 0, 100);
    }

    public void SetEnvironmentHealth(float health)
    {
        environmentalHealth = Mathf.Clamp(health, minHealth, maxHealth);
        onEnvironmentalHealthChange?.Invoke((int)environmentalHealth); // Notify any listeners of the updated health value
    }

    public void NewDayCycle()
    {
        SpawnTrash(trashSpawnCount); // Add trash at the start of a new day
        CalculateEnvironmentHealth(); // Update health for any new trash added
    }

    public int GetTrashCount()
    {
        return TrashCount;
    }
}
