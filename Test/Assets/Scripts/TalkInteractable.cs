using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkInteractable : Interactable
{
    public override void Interact(Character character)
    {
        Debug.Log("You talked to me!");
    }

}
