
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/Dialogue/Dialogue")]
public class DialogueContainer : ScriptableObject
{
    public List<string> DialogueLines;
    public Actor actor;

  
}
