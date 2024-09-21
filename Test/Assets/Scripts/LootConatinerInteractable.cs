using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootContainerInteractable : Interactable
{
     [SerializeField] GameObject closed;
    [SerializeField] GameObject opened;
    [SerializeField] bool open;


    public override void Interact(Character character)
    {
        if(open==false)
        {
            open=true;
            closed.SetActive(false);
            opened.SetActive(true);
        }
    }

    

}
