using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    private void Awake(){
        instance = this;
    }
    void Start()
    {
        DatabaseManager dbManager = FindObjectOfType<DatabaseManager>();
        inventoryContainer.LoadItemsFromDatabase(dbManager);
    }
    public GameObject player;
    public ItemContainer inventoryContainer;
    public ItemDragAnDropContainer dragAndDropController;
    public DayNightController TimeController;
    public DialogueSystem dialogueSystem;


    public void AddItemToInventory(Item item, int count)
    {
        if (inventoryContainer != null)
        {
            inventoryContainer.Add(item, count);
            Debug.Log($"Added {count} of {item.Name} to the inventory.");
        }
        else
        {
            Debug.LogWarning("No inventory container attached to the GameManager.");
        }
    }

}
