using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public bool stackable;
    public Sprite icon;
    public ToolAction OnAction;
    public ToolAction onTileMapAction;
    public ToolAction onItemUsed;
    public Crop crop;// make crop its own subclass of item
    
}
