using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

  
    public GameObject PickUpItemPrefab;
	PlayerSaveData playerSaveData;
	public static GameManager instance { get; private set; }
    private DatabaseManager dbManager;
    public Item[] itemAssets;
    public ItemContainer inventoryContainer;

	private void Awake()
	{
        dbManager = FindObjectOfType<DatabaseManager>();
		// Ensure only one instance of GameManager exists
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}

		instance = this; // Set the singleton instance
		//DontDestroyOnLoad(gameObject);
		
    }

    void Start()
    {
		Debug.LogError("UserID :"+ userid);

		nameMainMenuScene = "MainMenuScene";

		foreach (var slot in inventoryContainer.slots) // just to make sure its cleared for the testings
		{
			slot.Clear();
		}


		switch (DatabaseManager.instance.getRoleID(userid))
        {
            case 1: // Player
                LoadData();
                LoadPlayerInventory();
                break;

            default: // Dev and Admin (and any other role)
                LoadDevInventory();
                break;
        }

        if (!startTut)
                {
                    tutorialManager.LoadTutfromID(1);
                    startTut = true;
                }

    }
    public GameObject player;
    public ItemDragAnDropContainer dragAndDropController;
    public DayNightController TimeController;
    public DialogueSystem dialogueSystem; 
    public TutorialManager tutorialManager;

    public EnvironmentManager environmentManager;

	public SaveLoadSystem saveLoadSystem;

	public static int userid=1;
   
    string encrypteddata;
    
    bool inventoryTut = false;
    bool startTut= false;

	[Header("Transition")]
	[SerializeField] string nameMainMenuScene;



    void LoadPlayerInventory()
{
    // Check if the player has an inventory linked to their user account
    if (DatabaseManager.instance.InventoryExists(userid))
    {
        // Load the player's inventory from the database if necessary
        var playerItems = DatabaseManager.instance.LoadPlayerInventory(userid);
        foreach (var item in playerItems)
        {
            dbManager.AddSingleItemToInventory(item.Name, "InventoryItems", item.Amount);
            Debug.Log($"Loaded {item.Amount} of {item.Name} from database.");
        }
    }
    else
    {
        // Load default inventory if no inventory exists
        LoadDefaultInventory();
    }
}

void LoadDevInventory()
{
    // Load a predefined inventory for dev/admin
    foreach (var item in itemAssets) // Iterate over your item assets
    {
        AddItemToInventory(item, 999); // You can adjust the quantity as needed
        Debug.Log($"Added {item.Name} to the inventory with amount 999.");
    }

    player.GetComponent<Currency>().gold = 99999;

    player.GetComponent<Currency>().UpdateGold();
}

void LoadDefaultInventory()
{

    Debug.Log("Default inventory is empty lol");

}

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
    /// must populate playersavedata somehow.


    public void SaveData()
    {
        Debug.Log("Save datathing run");
		playerSaveData = new PlayerSaveData();

		playerSaveData.airquality = environmentManager.airQuality;
        playerSaveData.soilHealth = environmentManager.soilHealth;
        playerSaveData.EnvironmentalHealth = environmentManager.environmentalHealth;

        playerSaveData.GoldAmount = player.GetComponent<Currency>().gold;
        playerSaveData.tutinv = inventoryTut;
        playerSaveData.tutstart = startTut;

        encrypteddata = saveLoadSystem.Save(playerSaveData);

        DatabaseManager.instance.SaveEncrypteddata(encrypteddata,userid);
    }

	void LoadData()
	{
        string encrypteddata = DatabaseManager.instance.LoadEncrypteddata(userid);

		if (string.IsNullOrEmpty( encrypteddata))
        {
            Debug.LogError("Data Not found");
            return;
        }    
        playerSaveData = saveLoadSystem.Load(encrypteddata);

        Debug.LogWarning(playerSaveData.GoldAmount);

        environmentManager.airQuality = playerSaveData.airquality;
        environmentManager.soilHealth = playerSaveData.soilHealth;

        environmentManager.environmentalHealth = playerSaveData.EnvironmentalHealth;

        player.GetComponent<Currency>().gold = playerSaveData.GoldAmount;
        inventoryTut = playerSaveData.tutinv;
		startTut = playerSaveData.tutstart;

        player.GetComponent<Currency>().UpdateGold();

		/// put playerSave Data into other fields
	}


    public void LoadMainMenuScene()
{
    foreach (var slot in inventoryContainer.slots)
    {
        slot.Clear();
    }

    DatabaseManager.instance.SaveTradeDataToFile();

    DatabaseManager.instance.ClearTradeTable();

    SceneManager.LoadScene(nameMainMenuScene, LoadSceneMode.Single);
}
	internal void Checktut()
	{
        if (!tutorialManager.CheckIsActive())
        {
            if (!inventoryTut)
            {
                tutorialManager.LoadTutfromID(2);
                inventoryTut = true;
            }
        }
	}
}
