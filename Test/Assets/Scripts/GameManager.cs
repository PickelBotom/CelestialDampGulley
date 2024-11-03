using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public GameObject PickUpItemPrefab;



	PlayerSaveData playerSaveData;
	private void Awake(){
        instance = this;
    }
    void Start()
    {
		DatabaseManager dbManager = FindObjectOfType<DatabaseManager>();

        inventoryContainer.LoadItemsFromDatabase(dbManager);

       // LoadData();



        
    }
    public GameObject player;
    public ItemContainer inventoryContainer;
    public ItemDragAnDropContainer dragAndDropController;
    public DayNightController TimeController;
    public DialogueSystem dialogueSystem;
    public SaveLoadSystem saveLoadSystem;

	public static string userRole="blah";
    string encrypteddata;

	[Header("Transition")]
	[SerializeField] string nameMainMenuScene;

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

		SceneManager.LoadScene(nameMainMenuScene, LoadSceneMode.Single);
	}
}
