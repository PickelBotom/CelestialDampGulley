using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using UnityEngine.Tilemaps;
using System;
using Random = System.Random;
using TMPro;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text;
using static UnityEditor.Progress;
using Unity.VisualScripting;






#if UNITY_EDITOR
using UnityEditor;
#endif

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;
    private IDbConnection dbConnection;
    //public static DatabaseManager instance; // Singleton instance

    /// Dialogue stuff
    string DialogueTBName;
    Random rnum = new Random();
    int randDialogue;
    long DialogueTBSize;
    string DialogueStr;
    /// 

    ///// Trading stuff
    string TradeTBName;
   [SerializeField] List<Sprite> Iconlist;
	/////

	public static DatabaseManager instance { get; private set; }

    

	private void Awake()
	{
		// Ensure only one instance of DatabaseManager exists
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}

		instance = this; // Set the singleton instance
		DontDestroyOnLoad(gameObject); // Persist across scenes

		InitializeDatabase(); // Initialize the database connection
	}
	
    //void Awake()
	//   {
	//       if (instance == null)
	//       {
	//           instance = this;
	//           DontDestroyOnLoad(gameObject); // Keeps the database manager alive across scenes
	//       }
	//       else
	//       {
	//           Destroy(gameObject);
	//       }
	//   }

	private void InitializeDatabase()
	{
		// Example: Initialize your database connection
		// dbConnection = new SQLiteConnection("YourConnectionString");
		// dbConnection.Open();
		dbPath = Path.Combine(Application.persistentDataPath, "gameDatabase.db");

		DeleteDatabase();

		// Set up the database path
		// Create the database file if it does not exist
		if (!File.Exists(dbPath))
		{
			SqliteConnection.CreateFile(dbPath);
		}

		// Open the connection
		dbConnection = new SqliteConnection("URI=file:" + dbPath);
		dbConnection.Open();


		CreateTables();

		//LoadDialogueDataIntoTables();
		//LoadTradeDataIntoTables();
		LoadUserDataIntotables();
		LoadTutTableData();

		//PopulateItems();
		TestLoadItems();
		//AddSingleItemToInventory("WheatSeeds", "Items", 500);
		//AddSingleItemToInventory("CornSeeds", "Items", 500);

		// Example usage


		Sprite testSprite = Resources.Load<Sprite>("Art/Crop_Spritesheet");
		if (testSprite != null)
		{
			Debug.Log("Successfully loaded sprite manually: " + testSprite.name);
		}
		else
		{
			Debug.LogWarning("Failed to load sprite manually from Resources.");
		}

		// Close the connection when done
	}




	private void CreateTables()
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {

			// Roles table
			dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Roles (
                    RoleID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    UserType VARCHAR(25)
                )";
			dbCmd.ExecuteNonQuery();

			// Create Users table
			dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Username TEXT, 
                    Password TEXT, 
                    RoleID INTEGER,
                    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
                )";
            dbCmd.ExecuteNonQuery();
           

            // Tuttable
			dbCmd.CommandText = @" 
                CREATE TABLE IF NOT EXISTS TutTable (
                    TUTID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    TUTInfo TEXT
                )";
			dbCmd.ExecuteNonQuery();

            // Savedata table
			dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS SaveData (
                    SaveID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    EncryptedData TEXT,
                    UserID INTEGER,
                    FOREIGN KEY (UserID) REFERENCES Users(UserID) 
                )";
			dbCmd.ExecuteNonQuery();


			// Create Crops table without Yield column
			dbCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Crops (
                CropID INTEGER PRIMARY KEY AUTOINCREMENT, 
                Name TEXT, 
                GrowthTime INTEGER, 
                SellPrice INTEGER, 
                Description TEXT, 
                Icon BLOB,
                SpriteName TEXT,
                Stackable BOOLEAN
            )";
            dbCmd.ExecuteNonQuery();

            // Create InventoryItems table
            dbCmd.CommandText = @"
                ItemID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID INTEGER,
                    Name TEXT,
                    Category TEXT,
                    Effect TEXT,
                    CraftingRecipe TEXT,
                    SellPrice INTEGER,
                    Description TEXT,
                    Icon BLOB,
                    SpriteName TEXT,
                    Quantity INTEGER DEFAULT 0,
                    FOREIGN KEY (UserID) REFERENCES Users(UserID)
                )";
            dbCmd.ExecuteNonQuery();


// New Additions for the DB refactoring
            dbCmd.CommandText = @"
                CREATE TABLE Items (
                    ItemID INTEGER PRIMARY KEY,
                    Stackable BOOLEAN,
                    SellPrice INTEGER,
                    BuyPrice INTEGER

                )";
            dbCmd.ExecuteNonQuery();

            dbCmd.CommandText = @"
            CREATE TABLE ItemTypes (
                ItemTypeID INTEGER PRIMARY KEY,
                Type TEXT CHECK(Type IN ('Crops', 'Seeds', 'Tools'))
            )";
            dbCmd.ExecuteNonQuery();

            dbCmd.CommandText = @"
            CREATE TABLE Trade (
                    TradeID INTEGER PRIMARY KEY AUTOINCREMENT,
                    NPCID INTEGER,
                    ItemID INTEGER,
                    TradeType TEXT CHECK(TradeType IN ('buy', 'sell')), -- Specify if trade is a buy or sell
                    Quantity INTEGER NOT NULL,                          -- Track amount of items traded
                    TradeTimestamp DATETIME DEFAULT CURRENT_TIMESTAMP,  -- Log the trade time
                    FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
                    FOREIGN KEY (NPCID) REFERENCES NPC(NPCID)
            )";
            dbCmd.ExecuteNonQuery();

            dbCmd.CommandText = @"
            CREATE TABLE NPC (
                NPCID INTEGER PRIMARY KEY,
                NPCType TEXT NOT NULL
            )";

            dbCmd.ExecuteNonQuery();

            dbCmd.CommandText = @"
            CREATE TABLE NPC_Dial (
                NPCID INTEGER,
                DialID INTEGER,
                PRIMARY KEY (NPCID, DialID),
                FOREIGN KEY (NPCID) REFERENCES NPC(NPCID),
                FOREIGN KEY (DialID) REFERENCES Dialogue(DialID)
            )";
            dbCmd.ExecuteNonQuery();

            dbCmd.CommandText = @"
            CREATE TABLE Dialogue (
                DialID INTEGER PRIMARY KEY,
                Info TEXT NOT NULL
            )";
            dbCmd.ExecuteNonQuery();


        }
    }

    public int getRoleID(int userid)
    {
        int roleid = 0;
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = $"SELECT RoleID" +
                $" FROM Users " +
                $"WHERE UserID = {userid}";

            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    roleid = int.Parse(reader["RoleID"].ToString());
                }
                else
                {
                    Debug.LogError($" something else went wrong in the pulling of Role ID");
                }
            }
        }
        return roleid;
    }




    ///////////// START DIALOGUE CODE /////////////
    public void PopulateList(DialogueContainer currentDialogue)
    {
        currentDialogue.DialogueLines.Clear();
        DialogueTBName = "DialogueTB" + currentDialogue.actor.TBName;

        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = $"SELECT COUNT(*) FROM {DialogueTBName} ";
            DialogueTBSize = (long)dbCmd.ExecuteScalar();
            Debug.LogError("Dialogue size : " + (int)DialogueTBSize);
        }
        if (DialogueTBSize > 1)
        {
            randDialogue = rnum.Next(1, (int)DialogueTBSize + 1);
        }
        else
        {
            randDialogue = 1;
        }

        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = $"SELECT * FROM {DialogueTBName} WHERE ID = {randDialogue} ";
            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    DialogueStr = reader["Info"].ToString();
                    currentDialogue.DialogueLines = new List<string>(DialogueStr.Split("."));

                }
                else
                {
                    Debug.LogError($"Database table \"{DialogueTBName}\" not found! or something else went wrong in the pulling of dialogue");
                }

            }
        }

    }


    public void InsertDialogueData(string temp, string name)
    {
        string tbname = "DialogueTB" + name;
        //Debug.Log(tbname);

        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            //byte[] iconBytes = ConvertSpriteToByteArray(icon); // Convert sprite to byte array

            dbCmd.CommandText = $"INSERT INTO {tbname} (Info) " +
                                $"VALUES (@Info)";

            AddParameterWithValue(dbCmd, "@Info", temp);
			dbCmd.ExecuteNonQuery();
		}
			//using (IDbCommand dbCmd = dbConnection.CreateCommand())
   //     {
   //         dbCmd.CommandText = $"INSERT INTO {tbname} (Info) VALUES ('{temp}')"; // caused issues with the word "don't"
   //         dbCmd.ExecuteNonQuery();
   //     }
    }

    /////////////////END DIALOGUE CODE////////////////////// 




    ///////////// START TRADING  CODE /////////////
    public void PopulateTradeFields(TradingSlot TS, string TbNameCaller,int id)
    {
        TradeTBName = "TradeTB" + TbNameCaller;

		if (TS != null)
		{
			Debug.LogError("TS was found");
		}
		if (TS == null)
		{
			Debug.LogError("TS is null");
			return;
		}
		// database pull :)
		using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = $"SELECT * FROM {TradeTBName} WHERE ID = '{id}'"; 
            using (IDataReader reader = dbCmd.ExecuteReader())
            {
              //  Debug.LogWarning("Reading database : "+reader.Read());
                if (reader.Read())
                {
                    Debug.LogWarning("Reader amount" + reader["Amount"].ToString());

                    TS.Itname = reader["Name"].ToString();
                    TS.amount = int.Parse(reader["Amount"].ToString());
                    TS.sellp = int.Parse(reader["SellPrice"].ToString());
                    TS.buyp = int.Parse(reader["BuyPrice"].ToString());

                    // Convert byte array back to sprite
                    if (reader["Icon"] != DBNull.Value) // Check if the icon column is not null
                    {
                        byte[] iconBytes = (byte[])reader["Icon"];
                        TS.icon = ConvertByteArrayToSprite(iconBytes); // Convert BLOB to Sprite
                    }
                    else
                    {
                        Debug.LogWarning($"Icon for crop '{TS.Itname}' is null.");
                    }

                    //TS.ItemName.text = "Name: " + reader["Name"].ToString();
                    //               TS.AmountItems.text = "Amount: " + reader["Amount"].ToString();
                    //               TS.SellPrice.text = "Sell - " + reader["SellPrice"].ToString();
                    //               TS.Buyprice.text = "Buy - " + reader["BuyPrice"].ToString();

                    //               // Convert byte array back to sprite
                    //               if (reader["Icon"] != DBNull.Value) // Check if the icon column is not null
                    //               {
                    //                   byte[] iconBytes = (byte[])reader["Icon"];
                    //                   TS.ItemImg.sprite = ConvertByteArrayToSprite(iconBytes); // Convert BLOB to Sprite
                    //               }
                    //               else
                    //               {
                    //                   Debug.LogWarning($"Icon for crop '{TS.name}' is null.");
                    //               }
                    //
                    //public Image ItemImg;
                    //public TMP_Text ItemName;
                    //public TMP_Text AmountItems;
                    //public TMP_Text Buyprice;
                    //public TMP_Text SellPrice;

                }
                else
                {
					Debug.LogWarning($"'{id}' not found null.");
				}

			}	
        }

	}
//	Sprite icon,
	public void InsertTradeItems(string name, int amount,int buyp, int sellp,string TBname, Sprite icon)
	{
		TradeTBName = "TradeTB" + TBname;
		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			byte[] iconBytes = ConvertSpriteToByteArray(icon); // Convert sprite to byte array

			dbCmd.CommandText = $"INSERT INTO {TradeTBName} (Name, Amount,BuyPrice,SellPrice, Icon) " +
								$"VALUES (@Name, @Amount, @BuyPrice, @SellPrice, @Icon)";

			AddParameterWithValue(dbCmd, "@Name", name);
			AddParameterWithValue(dbCmd, "@Amount", amount);			
			AddParameterWithValue(dbCmd, "@BuyPrice", buyp);
			AddParameterWithValue(dbCmd, "@SellPrice", sellp);
			AddParameterWithValue(dbCmd, "@Icon", iconBytes);
			dbCmd.ExecuteNonQuery();

		}
	}
    //Name VARCHAR(25),
    //Amount INTEGER,
    //BuyPrice INTEGER,
    //SellPrice INTEGER,
    //Icon BLOB

    /////////////////END TRADING CODE////////////////////// 


    /////////////////Start Tutorial CODE////////////////////// 

    void AddTut(string tutinfo)
    {
            
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {

                dbCmd.CommandText = $"INSERT INTO TutTable (TUTInfo) " +
                                    $"VALUES (@Info)";

                AddParameterWithValue(dbCmd, "@Info", tutinfo);
                dbCmd.ExecuteNonQuery();
            }
      

    }
	///	/////////////////END Tutorial CODE////////////////////// 



	public void AddUser(string username, string password, int role)
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = $"INSERT INTO Users (Username, Password, RoleID) " +
                $"VALUES (@Username,@Password,@RoleID)";

			AddParameterWithValue(dbCmd, "@Username", username);
			AddParameterWithValue(dbCmd, "@Password", HashPassword (password) ); // Convert bool to integer
			AddParameterWithValue(dbCmd, "@RoleID", role);
			dbCmd.ExecuteNonQuery();
        }
    }

    public void AddUserType(string type)
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
			dbCmd.CommandText = $"INSERT INTO Roles (UserType) VALUES ('{type}')";
			dbCmd.ExecuteNonQuery();
		}		
    }


    public void AddItem(string name, Sprite icon, bool isStackable)
{
    using (IDbCommand dbCmd = dbConnection.CreateCommand())
    {
        byte[] iconBytes = ConvertSpriteToByteArray(icon); // Convert sprite to byte array

        dbCmd.CommandText = $"INSERT INTO Items (Name, Stackable, Icon) " +
                            $"VALUES (@Name, @Stackable, @Icon)";

        AddParameterWithValue(dbCmd, "@Name", name);
        AddParameterWithValue(dbCmd, "@Stackable", isStackable ? 1 : 0); // Convert bool to integer
        AddParameterWithValue(dbCmd, "@Icon", iconBytes);

        dbCmd.ExecuteNonQuery();

        // Save the item as a ScriptableObject asset
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.Name = name;
        newItem.stackable = isStackable;
        newItem.icon = icon;

        SaveItemAsset(newItem, icon); // Save the asset
    }
}


    public void AddCrop(Crop crop, Sprite icon, bool isStackable)
{
    using (IDbCommand dbCmd = dbConnection.CreateCommand())
    {
        byte[] iconBytes = ConvertSpriteToByteArray(icon); // Convert sprite to byte array

        dbCmd.CommandText = $"INSERT INTO Crops (Name, GrowthTime, SellPrice, Description, Icon, SpriteName, Stackable) " +
                            $"VALUES (@Name, @GrowthTime, @SellPrice, @Description, @Icon, @SpriteName, @Stackable)";

        // Add parameters
        AddParameterWithValue(dbCmd, "@Name", crop.name);
        AddParameterWithValue(dbCmd, "@GrowthTime", crop.timeToGrow);
        AddParameterWithValue(dbCmd, "@SellPrice", crop.SellPrice);
        AddParameterWithValue(dbCmd, "@Description", crop.Description);
        AddParameterWithValue(dbCmd, "@Icon", iconBytes);
        AddParameterWithValue(dbCmd, "@SpriteName", icon.name);
        AddParameterWithValue(dbCmd, "@Stackable", isStackable ? 1 : 0); // Convert bool to integer

        dbCmd.ExecuteNonQuery();

        // Save Crop as ScriptableObject asset
        SaveCropAsset(crop, icon);
    }
}

    public void AddInventoryItem(Item item, string spriteName)
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            byte[] iconBytes = ConvertSpriteToByteArray(item.icon); // Convert sprite to byte array

            dbCmd.CommandText = $"INSERT INTO InventoryItems (Name, Category, Effect, CraftingRecipe, SellPrice, Description, Icon, SpriteName) " +
                                $"VALUES (@Name, @Category, @Effect, @CraftingRecipe, @SellPrice, @Description, @Icon, @SpriteName)";

            AddParameterWithValue(dbCmd, "@Name", item.Name);
            AddParameterWithValue(dbCmd, "@Category", item.Category);
            AddParameterWithValue(dbCmd, "@SellPrice", item.SellPrice);
            AddParameterWithValue(dbCmd, "@Description", item.Description);
            AddParameterWithValue(dbCmd, "@Icon", iconBytes); // Assign the byte array to the parameter
            AddParameterWithValue(dbCmd, "@SpriteName", spriteName); // Store the sprite name for future reference

            dbCmd.ExecuteNonQuery();
        }
    }

    public void SaveCropAsset(Crop crop, Sprite icon)
    {
#if UNITY_EDITOR
        // Create a new Crop asset and save it
        Crop newCrop = ScriptableObject.CreateInstance<Crop>();
        newCrop.name = crop.name;
        newCrop.timeToGrow = crop.timeToGrow;
        newCrop.SellPrice = crop.SellPrice;
        newCrop.Description = crop.Description;
        newCrop.icon = icon; // Set the icon

        // Create the asset path
        string path = "Assets/Resources/Crops/" + crop.name + ".asset";
        AssetDatabase.CreateAsset(newCrop, path);
        AssetDatabase.SaveAssets();
        Debug.Log($"Crop asset created: {path}");
#endif
    }

    public void SaveItemAsset(Item item, Sprite icon)
{
#if UNITY_EDITOR
    // Create a new Item asset and save it
    Item newItem = ScriptableObject.CreateInstance<Item>();
    newItem.Name = item.Name; // Set the name
    newItem.stackable = item.stackable; // Set stackable property
    newItem.SellPrice = item.SellPrice; // Set the sell price
    newItem.Description = item.Description; // Set the description
    newItem.icon = icon; // Set the icon

    // Create the asset path
    string path = "Assets/Resources/Items/" + item.Name + ".asset"; // Adjust path as necessary
    AssetDatabase.CreateAsset(newItem, path);
    AssetDatabase.SaveAssets();
    Debug.Log($"Item asset created: {path}");
#endif
}

    public List<Crop> GetAllCrops()
{
    List<Crop> crops = new List<Crop>();
    using (IDbCommand dbCmd = dbConnection.CreateCommand())
    {
        dbCmd.CommandText = "SELECT * FROM Crops"; // Query the crops table
        using (IDataReader reader = dbCmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Crop crop = ScriptableObject.CreateInstance<Crop>(); // Create instance of Crop
                crop.name = reader["Name"].ToString(); // Retrieve crop name
                crop.timeToGrow = int.Parse(reader["GrowthTime"].ToString()); // Retrieve growth time
                crop.SellPrice = int.Parse(reader["SellPrice"].ToString()); // Retrieve sell price
                crop.Description = reader["Description"].ToString(); // Retrieve description
                crop.Stackable = reader["Stackable"].ToString() == "1"; // Assign stackable property

                // Convert byte array back to sprite
                if (reader["Icon"] != DBNull.Value) // Check if the icon column is not null
                {
                    byte[] iconBytes = (byte[])reader["Icon"];
                    crop.icon = ConvertByteArrayToSprite(iconBytes); // Convert BLOB to Sprite
                }
                else
                {
                    Debug.LogWarning($"Icon for crop '{crop.name}' is null.");
                }

                crops.Add(crop); // Add crop to the list
            }
        }
    }
    return crops; // Return the list of crops
}

    public List<Item> GetAllInventoryItems()
    {
        List<Item> items = new List<Item>();
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "SELECT * FROM InventoryItems";
            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Item item = ScriptableObject.CreateInstance<Item>(); // Create instance of Item
                    item.Name = reader["Name"].ToString();
                    item.Category = reader["Category"].ToString();
                    item.SellPrice = int.Parse(reader["SellPrice"].ToString());
                    item.Description = reader["Description"].ToString();

                    // Handle Icon if needed
                    byte[] iconBytes = (byte[])reader["Icon"];
                    item.icon = ConvertByteArrayToSprite(iconBytes); // Convert BLOB to Sprite

                    // Add the item to the inventory (you can choose the count as needed)
                    GameManager.instance.AddItemToInventory(item, 1); // Add 1 of each item
                    items.Add(item);
                }
            }
        }
        return items;
    }

    public List<Item> GetAllItems()
    {
        List<Item> items = new List<Item>();
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "SELECT * FROM Items";
            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Item item = ScriptableObject.CreateInstance<Item>(); // Create instance of Item
                    item.Name = reader["Name"].ToString();
                    item.stackable = reader["Stackable"].ToString() == "1"; // Assign stackable property

                    // Handle Icon if needed
                    if (reader["Icon"] != DBNull.Value)
                    {
                        byte[] iconBytes = (byte[])reader["Icon"];
                        item.icon = ConvertByteArrayToSprite(iconBytes); // Convert BLOB to Sprite
                    }
                    
                    // Add the item to the inventory
                    GameManager.instance.AddItemToInventory(item, 1); // Add 1 of each item
                    items.Add(item);
                }
            }
        }
        return items;
    }

    private void AddColumnIfNotExist(string tableName, string columnName, string columnType)
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            // Check if the column exists in the table
            dbCmd.CommandText = $"PRAGMA table_info({tableName})";
            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                bool columnExists = false;
                while (reader.Read())
                {
                    if (reader["name"].ToString() == columnName)
                    {
                        columnExists = true;
                        break;
                    }
                }

                // If the column does not exist, add it
                if (!columnExists)
                {
                    reader.Close(); // Close the reader before executing a new command
                    dbCmd.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType}";
                    dbCmd.ExecuteNonQuery();
                    Debug.Log($"Added column {columnName} to table {tableName}");
                }
            }
        }
    }

    public void AddSingleItemToInventory(string itemName, string tableName, int quantity)
    {
        // Fetch the item from the specified table
        Item item = GetItemByNameAndTable(itemName, tableName);

        if (item != null)
        {
            GameManager.instance.AddItemToInventory(item, quantity);
            Debug.Log($"Added {item.Name} (x{quantity}) from {tableName} to the inventory.");
        }
        else
        {
            Debug.LogWarning($"Item '{itemName}' not found in the {tableName} table.");
        }
    }

    // Method to fetch an item by its name from the specified table
    public Item GetItemByNameAndTable(string itemName, string tableName)
    {
        Item item = null;
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = $"SELECT * FROM {tableName} WHERE Name = @itemName";
            IDbDataParameter param = dbCmd.CreateParameter();
            param.ParameterName = "@itemName";
            param.Value = itemName;
            dbCmd.Parameters.Add(param);

            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Log to check if the item is being created
                    Debug.Log($"Fetching item from DB: {itemName}");

                    // Create a new instance of the Item ScriptableObject
                    item = ScriptableObject.CreateInstance<Item>();
                    item.Name = reader["Name"].ToString();
                    item.SellPrice = int.Parse(reader["SellPrice"].ToString());
                    item.Description = reader["Description"].ToString();

                    Debug.Log($"Item created: {item.Name}, SellPrice: {item.SellPrice}");

                    // Fetch the SpriteName and load it from the spritesheet
                    string spriteName = reader["SpriteName"].ToString();
                    item.icon = LoadSpriteFromSheet(spriteName); // Ensure this works correctly

                    if (item.icon != null)
                    {
                        Debug.Log($"Icon loaded successfully: {spriteName}");
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to load icon: {spriteName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Item '{itemName}' not found in table '{tableName}'");
                }
            }
        }
        return item;
    }

    private void DeleteDatabase()
    {
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
            Debug.Log("Database deleted successfully.");
        }
        else
        {
            Debug.Log("No file to delete.");
        }
    }

    private byte[] ConvertSpriteToByteArray(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("Sprite is null, returning empty byte array.");
            return new byte[0];
        }

        Texture2D texture = sprite.texture;

        // Make sure the texture is readable
        if (!texture.isReadable)
        {
            Debug.LogError("Texture is not readable, cannot convert to byte array.");
            return new byte[0];
        }

        // Convert the Texture2D to a byte array
        byte[] bytes = texture.EncodeToPNG();

        if (bytes.Length == 0)
        {
            Debug.LogError("Failed to encode texture to byte array.");
        }

        return bytes;
    }

    private Sprite ConvertByteArrayToSprite(byte[] byteArray)
    {
        if (byteArray == null || byteArray.Length == 0)
        {
            Debug.LogWarning("Byte array is null or empty, returning null sprite.");
            return null;
        }

        // Create a Texture2D from the byte array
        Texture2D texture = new Texture2D(2, 2); // Create a texture with a small default size
        bool isLoaded = texture.LoadImage(byteArray); // Load the image from the byte array

        if (!isLoaded)
        {
            Debug.LogError("Failed to load texture from byte array.");
            return null;
        }

        // Create a new sprite from the texture
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        if (sprite != null)
        {
            Debug.Log($"Sprite successfully created: {sprite.name}, Texture size: {texture.width}x{texture.height}");
        }
        else
        {
            Debug.LogError("Failed to create sprite from texture.");
        }

        return sprite;
    }

    //public void PopulateItems()
    //{
    //    // Load sprites from Resources/Art folder
    //    Sprite[] cropSprites = Resources.LoadAll<Sprite>("Art/Crop_Spritesheet");


    //    AddItem("Corn Seeds", cropSprites[108], true); // Assign first sprite
    //    AddItem("Wheat Seeds", cropSprites[61], true); // Assign second sprite
        
    //}

    public Sprite LoadSpriteFromSheet(string spriteName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Assets/Resources/Art");
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == spriteName)
            {
                return sprite;
            }
        }
        Debug.LogWarning($"Sprite with name {spriteName} not found in spritesheet.");
        return null;
    }

    void TestLoadCrops()
    {
        List<Crop> crops = GetAllCrops();

        foreach (Crop crop in crops)
        {
            string cropInfo = $"Loaded Crop: {crop.name}";

            // Check if icon is null before accessing its properties
            if (crop.icon != null)
            {
                cropInfo += $", Sprite: {crop.icon.name}";
            }
            else
            {
                cropInfo += ", Sprite: null";
            }

            Debug.Log(cropInfo);
        }
    }
    void TestLoadItems()
    {
        List<Item> items = GetAllItems();

        foreach (Item item in items)
        {
            string itemInfo = $"Loaded Item: {item.Name}";

            // Check if icon is null before accessing its properties
            if (item.icon != null)
            {
                itemInfo += $", Sprite: {item.icon.name}";
            }
            else
            {
                itemInfo += ", Sprite: null";
            }

            Debug.Log(itemInfo);
        }
    }

    private void AddParameterWithValue(IDbCommand command, string parameterName, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = value ?? System.DBNull.Value; // Use DBNull.Value for nulls
        command.Parameters.Add(parameter);
    }


	/////////////////////////////// DIALOGUE ENTRIES ////////////////////////
	void LoadDialogueDataIntoTables()
	{
		/////////// TUT ////////////
		InsertDialogueData("REDUCE!.REUSE!.RECYCLE!", "Tut");

		InsertDialogueData("Littering has several negative effects on the environment:." +
			"Air Pollution: Litter releases harmful gases, including methane, which contributes to air pollution." +
			"Water Pollution: Litter can end up in rivers, lakes, and oceans, harming marine life." +
			"Death of Animals: Wildlife can ingest or get entangled in litter. " +
			"Spread of Disease and Infection: Improperly discarded trash can harbour bacteria and diseases." +
			"So Stop Littering! To save the dogs, cats, turtles and yourself", "Tut");

		InsertDialogueData("A reminder press the WASD Keys to move." +
			"Press the TAB Key to open the inventory." +
			"Use the scroll wheel to select items in your toolbar(The bar at the bottom of your screen)!", "Tut");

		InsertDialogueData("I don't like litter!." +
			"It makes the place unsightly." +
			"You best pick it up and put it in the trash can!." +
			"You do not want to carry trash all day do you and I will pay you just put it in the can", "Tut");

		InsertDialogueData("Talk to people around town to get information." +
			"Also if you want to buy stuff just go to interact with the chest to trade using the right mouse button", "Tut");
		/////////// WOOD ///////////
		InsertDialogueData("If we don't Deforestaion right there won't be no trees left." +
			"Without trees and stuff we can't breathe!", "Wood");

		InsertDialogueData("We keep chopping trees to expand our farms!." +
			"We losing trees cause government ain't governing right." +
			"They corrupt or they don't know enough about them trees!", "Wood");

		InsertDialogueData("We need to do something called Agroforestry." +
			"Its when you integrat trees with crops and livestock to enhance biodiversity", "Wood");
		/////////// TOOL ///////////
		InsertDialogueData("Composters are Tools for making compost on-site." +
			"It can reduce waste and provide organic matter to enhance soil fertility!", "Tool");

		InsertDialogueData("Soil Testing Kits areEssential for assessing soil health and nutrient levels." +
			"They allow farmers to tailor their fertilization practices and reduce chemical inputs", "Tool");

		InsertDialogueData("Seed dibblers help in planting seeds at the correct depth and spacing." +
			" Reducing seed waste :)", "Tool");

		/////////// SEED ///////////
		InsertDialogueData("Rotating different crops each season helps prevent soil depletion." +
			"Reduces pest and disease buildup." +
			"And surprisingly improves soil fertility", "Seed");

		InsertDialogueData("Growing certain plants together can enhance growth. Deter pests and improve pollination." +
			" For example, planting marigolds alongside vegetables can repel harmful insects", "Seed");

		InsertDialogueData("Sometimes Incorporating native plant varieties can help support local ecosystems." +
			"By attracting beneficial insects and enhance resilience to local pests and diseases", "Seed");
		/////////// STONE ///////////
		InsertDialogueData("Using stones or gravel as mulch can help retain soil moisture." +
			"Reduce erosion.Suppress weed growth." +
			"It also helps regulate soil temperature", "Stone");

		InsertDialogueData("Building dry stone walls creates habitat for wildlife and manage soil and water." +
			"They can create microclimates and protect crops from wind", "Stone");

		InsertDialogueData("Incorporate stones into crop rotation systems.It can improve soil structure and fertility." +
			"Especially when using rocks that release minerals over time", "Stone");
        Debug.Log("Dialogue loaded");
	}
    /////////////////////////////// END DIALOGUE ENTRIES ///////////////////////////


    void LoadTutTableData()
    {
        AddTut("Use the WASD Keys to move around.\n Use the TAB key to open the Inventory.\n Use the Esc key to open the menu");
		AddTut("Right Click on the items to drag and drop them into the top row to add them to the toolbar.\n You can also Right Click with a selected Item to interact with the world.");
	}


    /////////////////////////////// START TRADE ENTRIES ///////////////////////////

    void LoadTradeDataIntoTables()
    {
        // missing sprite to byte
        //InsertTradeItems(string name, int amount, int buyp, int sellp, string TBname)
        if (Iconlist[0] != null)
        {
            Debug.LogError("Icons found");
          
        }
		Sprite cropSprites = Resources.Load<Sprite>("Art/roguelikeitems/roguelikeitems_58");

		InsertTradeItems("Trash", 1, 0, 5, "Tut", cropSprites);
        InsertTradeItems("Trash", 5, 0, 30, "Tut", cropSprites);
        InsertTradeItems("Trash", 10, 0, 75, "Tut", cropSprites);


    }

	/////////////////////////////// END TRADE ENTRIES /////////////////////////////





	private void LoadUserDataIntotables()
	{

        FillUserTypes();

		AddUser("admin", "admin123", 3);
		AddUser("player", "p1", 1);
		AddUser("dev1", "d1", 2);
		AddUser("playertest2", "p2", 1);
		AddUser("P1", "x", 1);

	}

	private void FillUserTypes()
	{
        AddUserType("Player");
        AddUserType("Developer");
        AddUserType("Admin");
	}



	private string HashPassword(string password)
	{
		using (SHA256 sha256Hash = SHA256.Create())
		{
			byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				builder.Append(bytes[i].ToString("x2"));
			}
			return builder.ToString();
		}
	}

	internal void SaveEncrypteddata(string encD) // add userID 
	{
		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			dbCmd.CommandText = $"INSERT INTO InventoryItems (EncryptedData) " +
								$"VALUES (@EncryptedData)";

			AddParameterWithValue(dbCmd, "@EncryptedData", encD);
		}
	}

	internal string LaodEncrypteddata()
	{
        string encD="";

		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			
		}
        return encD;
	}

    public void InsertItem(int itemId, int tredId, bool stackable, int sellPrice, int buyPrice)
{
    using (IDbCommand dbCmd = dbConnection.CreateCommand())
    {
        dbCmd.CommandText = "INSERT INTO Items (ItemID, TredID, Stackable, SellPrice, BuyPrice) " +
                            "VALUES (@ItemID, @TredID, @Stackable, @SellPrice, @BuyPrice)";

        AddParameterWithValue(dbCmd, "@ItemID", itemId);
        AddParameterWithValue(dbCmd, "@TredID", tredId);
        AddParameterWithValue(dbCmd, "@Stackable", stackable);
        AddParameterWithValue(dbCmd, "@SellPrice", sellPrice);
        AddParameterWithValue(dbCmd, "@BuyPrice", buyPrice);

        dbCmd.ExecuteNonQuery();
    }
}
//New insert methods for DB refactoring
    public void InsertItemType(int itemTypeId, string type)
{
    using (IDbCommand dbCmd = dbConnection.CreateCommand())
    {
        dbCmd.CommandText = "INSERT INTO ItemTypes (ItemTypeID, Type) VALUES (@ItemTypeID, @Type)";

        AddParameterWithValue(dbCmd, "@ItemTypeID", itemTypeId);
        AddParameterWithValue(dbCmd, "@Type", type);

        dbCmd.ExecuteNonQuery();
    }
}

    public void InsertTrade(int tredId, int npcId, int itemId)
{
    using (IDbCommand dbCmd = dbConnection.CreateCommand())
    {
        dbCmd.CommandText = "INSERT INTO Trade (TredID, NPCID, ItemID) VALUES (@TredID, @NPCID, @ItemID)";

        AddParameterWithValue(dbCmd, "@TredID", tredId);
        AddParameterWithValue(dbCmd, "@NPCID", npcId);
        AddParameterWithValue(dbCmd, "@ItemID", itemId);

        dbCmd.ExecuteNonQuery();
    }
}

	internal string PopulateTutfield(int tutID)
	{

			string tutdata="";
		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			dbCmd.CommandText = $"SELECT * FROM TutTable WHERE TUTID = {tutID} ";
			using (IDataReader reader = dbCmd.ExecuteReader())
			{
				if (reader.Read())
				{
					tutdata = reader["TUTInfo"].ToString();
				}
				else
				{
					Debug.LogError($" something else went wrong in the pulling of Tutdata");
				}
			}
		}
        return tutdata;
	}
public void InsertNPC(int npcId, string npcType)
{
    using (IDbCommand dbCmd = dbConnection.CreateCommand())
    {
        dbCmd.CommandText = "INSERT INTO NPC (NPCID, NPCType) VALUES (@NPCID, @NPCType)";

        AddParameterWithValue(dbCmd, "@NPCID", npcId);
        AddParameterWithValue(dbCmd, "@NPCType", npcType);

        dbCmd.ExecuteNonQuery();
    }
}

public void InsertNPCDial(int npcId, int dialId)
{
    using (IDbCommand dbCmd = dbConnection.CreateCommand())
    {
        dbCmd.CommandText = "INSERT INTO NPC_Dial (NPCID, DialID) VALUES (@NPCID, @DialID)";

        AddParameterWithValue(dbCmd, "@NPCID", npcId);
        AddParameterWithValue(dbCmd, "@DialID", dialId);

        dbCmd.ExecuteNonQuery();
    }
}

public void InsertDialogue(int dialId, string info)
{
    using (IDbCommand dbCmd = dbConnection.CreateCommand())
    {
        dbCmd.CommandText = "INSERT INTO Dialogue (DialID, Info) VALUES (@DialID, @Info)";

        AddParameterWithValue(dbCmd, "@DialID", dialId);
        AddParameterWithValue(dbCmd, "@Info", info);

        dbCmd.ExecuteNonQuery();
    }
}

public void LogTrade(string tradeType, int itemID, int quantity, int totalValue)
{
    using (var connection = new SqliteConnection(dbPath))
    {
        try
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Trade (Type, ItemID, Quantity, TotalValue, TradeDate) 
                    VALUES (@type, @itemID, @quantity, @totalValue, @date);
                ";
                
                command.Parameters.AddWithValue("@type", tradeType); // "buy" or "sell"
                command.Parameters.AddWithValue("@itemID", itemID);
                command.Parameters.AddWithValue("@quantity", quantity);
                command.Parameters.AddWithValue("@totalValue", totalValue);
                command.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to log trade: " + ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }
}

public List<string> GetTradeData()
{
    List<string> tradeData = new List<string>();

    using (IDbConnection dbConnection = new SqliteConnection(dbPath))
    {
        dbConnection.Open();
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "SELECT * FROM Trade";
            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int tradeID = reader.GetInt32(0);
                    int itemID = reader.GetInt32(1);
                    string type = reader.GetString(2); // Assuming you added Type to Trade table
                    tradeData.Add($"TradeID: {tradeID}, ItemID: {itemID}, Type: {type}");
                }
            }
        }
    }

    return tradeData;
}



public void ClearTradeTable()
{
    using (IDbConnection dbConnection = new SqliteConnection(dbPath))
    {
        dbConnection.Open();
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "DELETE FROM Trade";
            dbCmd.ExecuteNonQuery();
        }
    }
}

public void SaveTradeDataToFile()
{
    List<string> tradeData = new List<string>();

    using (IDbConnection dbConnection = new SqliteConnection(dbPath))
    {
        dbConnection.Open();
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "SELECT ItemID, Amount, Type, Price FROM Trade";
            
            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                // Collect trade data entries
                while (reader.Read())
                {
                    int itemID = reader.GetInt32(0);
                    int amount = reader.GetInt32(1);
                    string type = reader.GetString(2);
                    int price = reader.GetInt32(3);
                    
                    tradeData.Add($"ItemID: {itemID}, Amount: {amount}, Type: {type}, Price: {price}");
                }
            }
        }
    }

    // Define file path and save data
    string filePath = Path.Combine(Application.persistentDataPath, "TradeSession.txt");
    File.WriteAllLines(filePath, tradeData);

    Debug.Log($"Trade session data saved to {filePath}");
}


}
