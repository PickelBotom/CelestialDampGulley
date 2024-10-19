using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering; // Required for Volume and VolumeProfile
using UnityEngine.Rendering.Universal; // Required for URP

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private Item trashItem; // Reference to your Trash item asset
    [SerializeField] private GameObject pickUpItemPrefab; // Reference to your PickUpItem prefab
    [SerializeField] private Vector3[] trashSpawnPoints;
    [SerializeField] private Transform trashParent;
    [SerializeField] private Image environmentTint; // Reference to the grey tint UI image
    [SerializeField] private Volume lightVolume; // Reference to the Volume component on Light2D prefab

    private const int trashSpawnCount = 3; // Number of trash items to spawn
    private float environmentalHealth = 100f;
    private const int minHealth = 0; // Minimum health
    private const int maxHealth = 100; // Maximum health
    private const float maxTintAlpha = 0.5f; // Maximum alpha for the grey tint

    private ColorAdjustments colorAdjustments; // Reference to ColorAdjustments settings

    public Action<int> onEnvironmentalHealthChange; // Action to notify when environmental health changes

    private void Start()
    {
        // Access the Color Adjustments effect directly from the Volume component
        if (lightVolume.sharedProfile != null)
        {
            // Use TryGet to access ColorAdjustments from the VolumeProfile
            if (lightVolume.sharedProfile.TryGet(out colorAdjustments))
            {
                // Successfully retrieved color adjustments settings
            }
            else
            {
                Debug.LogWarning("Color Adjustments not found in the Light2D Volume.");
            }
        }

        // Set the initial environmental health for testing
        environmentalHealth = 100f;

        // Invoke the health change to trigger the tint update
        onEnvironmentalHealthChange?.Invoke((int)environmentalHealth);

        // Update the tint based on the initial environmental health
        UpdateEnvironmentTint();

        // Spawn trash at the start of the game
        SpawnTrash(3);
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

        // Recalculate environmental health based on the number of trash items present
        CalculateEnvironmentHealth();
    }

    private void CalculateEnvironmentHealth()
    {
        int trashCount = trashParent.childCount;
        ModifyEnvironmentalHealth(-trashCount * 5); // Deduct 5 points for each trash item
    }

    public void PickUpTrash()
    {
        ModifyEnvironmentalHealth(10); // Increase health when trash is picked up
        CalculateEnvironmentHealth(); // Recalculate after picking up
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
        environmentalHealth += amount;
        environmentalHealth = Mathf.Clamp(environmentalHealth, minHealth, maxHealth);
        onEnvironmentalHealthChange?.Invoke((int)environmentalHealth);

        Debug.Log($"Environmental health changed by {amount}. Current health: {environmentalHealth}");

        // Adjust saturation based on health
        if (colorAdjustments != null)
        {
            float saturation = Mathf.Lerp(-100f, 0f, environmentalHealth / maxHealth); // Adjust saturation
            colorAdjustments.saturation.Override(saturation);
        }

        // Update the tint based on the current environmental health
        UpdateEnvironmentTint();
    }

    private void UpdateEnvironmentTint()
    {
        // If environmental health is above 80, no tint; below 80, gradually increase tint intensity
        if (environmentTint != null)
        {
            float tintAlpha = 0f;

            if (environmentalHealth < 80)
            {
                tintAlpha = Mathf.Lerp(maxTintAlpha, 0, environmentalHealth / 80f); // Gradually reduce alpha
            }

            Color tint = environmentTint.color;
            tint.a = tintAlpha;
            environmentTint.color = tint;
        }
    }

    public float GetCurrentSaturation()
{
    return Mathf.Lerp(-100f, 0f, environmentalHealth / maxHealth); // Adjust according to your saturation calculation
}

    public void NewDayCycle()
    {
        SpawnTrash(3);
    }
}
