using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using UnityEngine.Tilemaps;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;
    private IDbConnection dbConnection;
    public static DatabaseManager instance; // Singleton instance

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the database manager alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
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

        // Create tables if they don't exist
        CreateTables();

        PopulateItems();
        TestLoadItems();
        AddSingleItemToInventory("WheatSeeds", "Items", 500);
        AddSingleItemToInventory("CornSeeds", "Items", 500);

        // Example usage
        AddUser("admin", "admin123", "Admin");
		AddUser("player", "p1", "Player");
		AddUser("dev1", "d1", "Developer");
		AddUser("playertest2", "p2", "Player");

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
            // Create Users table
            dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Username TEXT, 
                    Password TEXT, 
                    Role TEXT
                )";
            dbCmd.ExecuteNonQuery();

            //// Add columns to Users table if they don't exist
            //AddColumnIfNotExist("Users", "PasswordHash", "TEXT");
            //AddColumnIfNotExist("Users", "Email", "TEXT");
            //AddColumnIfNotExist("Users", "AccessLevel", "TEXT");

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

            // Create Animals table
            dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Animals (
                    AnimalID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT, 
                    Type TEXT, 
                    Habitat TEXT, 
                    Benefit TEXT, 
                    Diet TEXT, 
                    Behavior TEXT, 
                    Description TEXT,
                    Icon BLOB
                )";
            dbCmd.ExecuteNonQuery();

            // Create InventoryItems table
            dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS InventoryItems (
                    ItemID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT, 
                    Category TEXT, 
                    Effect TEXT, 
                    CraftingRecipe TEXT, 
                    SellPrice INTEGER, 
                    Description TEXT, 
                    Icon BLOB,
                    SpriteName TEXT
                )";
            dbCmd.ExecuteNonQuery();

            // Create PlayerStats table
            dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS PlayerStats (
                    PlayerID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Username TEXT, 
                    Level INTEGER, 
                    ExperiencePoints INTEGER, 
                    Coins INTEGER, 
                    Reputation INTEGER, 
                    Achievements TEXT
                )";
            dbCmd.ExecuteNonQuery();

            // Create NPCs table
            dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS NPCs (
                    NPCID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT, 
                    Role TEXT, 
                    Affiliation TEXT, 
                    FriendshipLevel INTEGER, 
                    QuestsOffered TEXT, 
                    Backstory TEXT,
                    Icon BLOB
                )";
            dbCmd.ExecuteNonQuery();

            // Create Quests table
            dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Quests (
                    QuestID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Title TEXT, 
                    Description TEXT, 
                    Requirements TEXT, 
                    Rewards TEXT, 
                    Status TEXT, 
                    NPCID INTEGER, 
                    FOREIGN KEY(NPCID) REFERENCES NPCs(NPCID)
                )";
            dbCmd.ExecuteNonQuery();

            // Create Biomes table
            dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Biomes (
                    BiomeID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT, 
                    Description TEXT, 
                    FloraFauna TEXT, 
                    EnvironmentalHealth INTEGER, 
                    Climate TEXT, 
                    BuildingID INTEGER, 
                    Resources TEXT
                )";
            dbCmd.ExecuteNonQuery();

            // Create EcosystemMetrics table
            dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS EcosystemMetrics (
                    MetricID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    BiomeID INTEGER, 
                    MetricName TEXT, 
                    Value INTEGER, 
                    LastUpdated TEXT, 
                    ImpactFactors TEXT, 
                    FOREIGN KEY(BiomeID) REFERENCES Biomes(BiomeID)
                )";
            dbCmd.ExecuteNonQuery();

            // Create Buildings table
            dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Buildings (
                    BuildingID INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT, 
                    Function TEXT, 
                    Cost INTEGER, 
                    Requirements TEXT, 
                    Benefits TEXT, 
                    Description TEXT
                )";
            dbCmd.ExecuteNonQuery();

            // Create Items table
            dbCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Items (
                    ItemID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Stackable BOOLEAN,
                    Icon BLOB,
                    CropID INTEGER,
                    FOREIGN KEY(CropID) REFERENCES Crops(CropID)
                )";
            dbCmd.ExecuteNonQuery();
        }
    }

    public void AddUser(string username, string password, string role)
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = $"INSERT INTO Users (Username, Password, Role) VALUES ('{username}', '{password}', '{role}')";
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

    public void PopulateItems()
    {
        // Load sprites from Resources/Art folder
        Sprite[] cropSprites = Resources.LoadAll<Sprite>("Art/Crop_Spritesheet");


        AddItem("Corn Seeds", cropSprites[108], true); // Assign first sprite
        AddItem("Wheat Seeds", cropSprites[61], true); // Assign second sprite
        
    }

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
}
