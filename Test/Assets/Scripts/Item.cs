using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item")]
public class Item : ScriptableObject
{
    public int ItemID = 0; // Default ID
    public string Name = "New Item"; // Default name
    public bool stackable = true; // Default stackable
    public Sprite icon = null; // Default icon (can be set to a placeholder if you have one)
    public ToolAction OnAction = null; // Default to null
    public ToolAction onTileMapAction = null; // Default to null
    public ToolAction onItemUsed = null; // Default to null
    public Crop crop = null; // Default to null
    public int SellPrice = 0; // Default sell price
    public string Description = "No description"; // Default description
    public string Category = "General"; // Default category

    private void OnEnable()
    {
        // You can also set defaults here if needed, but initializing above is usually sufficient.
    }
}

