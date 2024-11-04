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
		DontDestroyOnLoad(gameObject);
		
    }

    void Start()
    {
		Debug.LogError("UserID :"+ userid);

		nameMainMenuScene = "MainMenuScene";


		//	DatabaseManager dbManager = FindObjectOfType<DatabaseManager>();

		tutorialManager.LoadTutfromID(1);

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

	public SaveLoadSystem saveLoadSystem;

	public static int userid;
   
    string encrypteddata;
    bool inventoryTut;

	[Header("Transition")]
	[SerializeField] string nameMainMenuScene;



    void loadPlayerInventory()
    {
        Debug.LogError("Player Loaded");
    }

    void LoadDevInventory()
    {
        Debug.LogError("Player Loaded");

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
        //encrypteddata = saveLoadSystem.Save(playerSaveData);

        //DatabaseManager.instance.SaveEncrypteddata(encrypteddata);
    }

	void LoadData()
	{
        string encrypteddata = DatabaseManager.instance.LaodEncrypteddata();

		if (string.IsNullOrEmpty( encrypteddata))
        {
            Debug.LogError("DataNot found");
            return;
        }    
        playerSaveData = saveLoadSystem.Load(encrypteddata);

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
