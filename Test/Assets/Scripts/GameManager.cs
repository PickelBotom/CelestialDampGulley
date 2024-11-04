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

	private void Awake()
	{
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


        
        //	DatabaseManager dbManager = FindObjectOfType<DatabaseManager>();
          LoadData();

        if (!startTut)
        {
            tutorialManager.LoadTutfromID(1);
            startTut = true;
        }


		switch (DatabaseManager.instance.getRoleID(userid))
        {
            case (1):// player
                {
                    loadPlayerInventory();

					break;
                }
			case (2):// Dev
				{
                    LoadDevInventory();
					break;
				}

			case (3):// Admin
				{
                    LoadDevInventory();
					break;
				}

		}



		//inventoryContainer.LoadItemsFromDatabase(dbManager);
        
       // LoadData();

    }
    public GameObject player;
    public ItemContainer inventoryContainer;
    public ItemDragAnDropContainer dragAndDropController;
    public DayNightController TimeController;
    public DialogueSystem dialogueSystem; 
    public TutorialManager tutorialManager;

    public EnvironmentManager environmentManager;

	public SaveLoadSystem saveLoadSystem;

	public static int userid=5;
   
    string encrypteddata;
    
    bool inventoryTut = false;
    bool startTut= false;

	[Header("Transition")]
	[SerializeField] string nameMainMenuScene;



    void loadPlayerInventory()
    {

        Debug.LogError("Player inventory Loaded");
    }

    void LoadDevInventory()
    {

        Debug.LogError("Dev inventory Loaded");
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
