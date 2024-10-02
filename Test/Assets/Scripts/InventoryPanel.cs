using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] ItemContainer inventory;
    [SerializeField] List<InventoryButton> buttons;
    // Start is called before the first frame update
    void Start()
    {
        SetIndex();
        Show();
    }
    void OnEnable()
    {
        Show();
    }

    // Update is called once per frame
    void Update()
    {
        Show();
    }
    public void Show(){
        for(int i = 0;i < inventory.slots.Count && i < buttons.Count; i++){
            if(inventory.slots[i].item == null){
                buttons[i].Clean();
            }
            else{
                buttons[i].Set(inventory.slots[i]);
            }
        }
    }
    private void SetIndex(){
        for(int i = 0; i < inventory.slots.Count && i < buttons.Count; i++){
            buttons[i].SetIndex(i);
        }
    }
}
