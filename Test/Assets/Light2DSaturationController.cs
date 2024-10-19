using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Required for Light2D

public class Light2DSaturationManager : MonoBehaviour
{
    private Light2D globalLight; // Reference to the Light2D component
    private float environmentalHealth = 100f;
    private const float minHealth = 0f;
    private const float maxHealth = 100f;
    
    private void Start()
    {
        globalLight = GetComponent<Light2D>();
        if (globalLight == null)
        {
            Debug.LogError("Light2D component not found on this GameObject.");
        }
    }

    public void UpdateEnvironmentalHealth(float health)
    {
        environmentalHealth = Mathf.Clamp(health, minHealth, maxHealth);
        UpdateLightColor();
    }

    private void UpdateLightColor()
    {
        // Adjust the light color based on the environmental health (0% health = grayscale, 100% health = normal color)
        float saturation = Mathf.Lerp(0.2f, 1f, environmentalHealth / maxHealth); // 0.2 to keep some color
        Color baseColor = globalLight.color;
        
        // Convert the color to grayscale based on saturation
        float grayscale = baseColor.grayscale;
        Color adjustedColor = Color.Lerp(new Color(grayscale, grayscale, grayscale), baseColor, saturation);

        globalLight.color = adjustedColor; // Apply new color to the light
    }
}

