using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class InventoryButton : MonoBehaviour
{

    [SerializeField] Image icon;
    [SerializeField] Text text;

    int myIndex;

    public void SetIndex(int index){
        myIndex = index;
    }

    public void Set(ItemSlot slot){
        icon.sprite = slot.item.icon;
        text.gameObject.SetActive(true);


        if (slot.item.stackable == true){
            text.gameObject.SetActive(true);
            text.text = slot.count.ToString();
        }
        else{
            text.gameObject.SetActive(false);
        }
    }

    public void Clean(){
        icon.sprite = null;
        icon.gameObject.SetActive(false);

        text.gameObject.SetActive(false);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
