using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/Dialogue/Dialogue")]
public class DialogueContainer : ScriptableObject
{
    public List<string> DialogueLines;
    public Actor actor;

  
}
