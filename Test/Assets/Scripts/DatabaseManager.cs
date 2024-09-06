using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;
    private IDbConnection dbConnection;

    void Start()
    {

        DeleteDatabase();
        
        // Set up the database path
        dbPath = Path.Combine(Application.persistentDataPath, "gameDatabase.db");

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

        // Example usage
        AddUser("admin", "admin123", "Admin");
        AddCrop("Wheat", 30);

        // Close the connection when done
        dbConnection.Close();
    }

    private void CreateTables()
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
           dbCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                Username TEXT, 
                Password TEXT, 
                Role TEXT
            )";
        dbCmd.ExecuteNonQuery();

        // Add PasswordHash column if it doesn't exist
        AddColumnIfNotExist("Users", "PasswordHash", "TEXT");

        // Add Email and AccessLevel columns if they don't exist
        AddColumnIfNotExist("Users", "Email", "TEXT");
        AddColumnIfNotExist("Users", "AccessLevel", "TEXT");

            // Create Crops table
            dbCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Crops (
                                  CropID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                  Name TEXT, 
                                  GrowthTime INTEGER, 
                                  Season TEXT, 
                                  Yield INTEGER, 
                                  SellPrice INTEGER, 
                                  Description TEXT)";
            dbCmd.ExecuteNonQuery();

            // Create Animals table
            dbCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Animals (
                                  AnimalID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                  Name TEXT, 
                                  Type TEXT, 
                                  Habitat TEXT, 
                                  Benefit TEXT, 
                                  Diet TEXT, 
                                  Behavior TEXT, 
                                  Description TEXT)";
            dbCmd.ExecuteNonQuery();

            // Create InventoryItems table
            dbCmd.CommandText = @"CREATE TABLE IF NOT EXISTS InventoryItems (
                                  ItemID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                  Name TEXT, 
                                  Category TEXT, 
                                  Effect TEXT, 
                                  CraftingRecipe TEXT, 
                                  SellPrice INTEGER, 
                                  Description TEXT)";
            dbCmd.ExecuteNonQuery();

            // Create PlayerStats table
            dbCmd.CommandText = @"CREATE TABLE IF NOT EXISTS PlayerStats (
                                  PlayerID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                  Username TEXT, 
                                  Level INTEGER, 
                                  ExperiencePoints INTEGER, 
                                  Coins INTEGER, 
                                  Reputation INTEGER, 
                                  Achievements TEXT)";
            dbCmd.ExecuteNonQuery();

            // Create NPCs table
            dbCmd.CommandText = @"CREATE TABLE IF NOT EXISTS NPCs (
                                  NPCID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                  Name TEXT, 
                                  Role TEXT, 
                                  Affiliation TEXT, 
                                  FriendshipLevel INTEGER, 
                                  QuestsOffered TEXT, 
                                  Backstory TEXT)";
            dbCmd.ExecuteNonQuery();

            // Create Quests table
            dbCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Quests (
                                  QuestID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                  Title TEXT, 
                                  Description TEXT, 
                                  Requirements TEXT, 
                                  Rewards TEXT, 
                                  Status TEXT, 
                                  NPCID INTEGER, 
                                  FOREIGN KEY(NPCID) REFERENCES NPCs(NPCID))";
            dbCmd.ExecuteNonQuery();

            // Create Biomes table
            dbCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Biomes (
                                  BiomeID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                  Name TEXT, 
                                  Description TEXT, 
                                  FloraFauna TEXT, 
                                  EnvironmentalHealth INTEGER, 
                                  Climate TEXT, 
                                  BuildingID INTEGER, 
                                  Resources TEXT)";
            dbCmd.ExecuteNonQuery();

            // Create EcosystemMetrics table
            dbCmd.CommandText = @"CREATE TABLE IF NOT EXISTS EcosystemMetrics (
                                  MetricID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                  BiomeID INTEGER, 
                                  MetricName TEXT, 
                                  Value INTEGER, 
                                  LastUpdated TEXT, 
                                  ImpactFactors TEXT, 
                                  FOREIGN KEY(BiomeID) REFERENCES Biomes(BiomeID))";
            dbCmd.ExecuteNonQuery();

            // Create Buildings table
            dbCmd.CommandText = @"CREATE TABLE IF NOT EXISTS Buildings (
                                  BuildingID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                  Name TEXT, 
                                  Function TEXT, 
                                  Cost INTEGER, 
                                  Requirements TEXT, 
                                  Benefits TEXT, 
                                  Description TEXT)";
            dbCmd.ExecuteNonQuery();

            // Create other related tables (Sessions, AccessLevels, etc.) following similar patterns
        }
    }

    public void AddUser(string username, string password, string role)
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = $"INSERT INTO Users (Username, PasswordHash, AccessLevel) VALUES ('{username}', '{password}', '{role}')";
            dbCmd.ExecuteNonQuery();
        }
    }

    public void AddCrop(string name, int growthTime)
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = $"INSERT INTO Crops (Name, GrowthTime) VALUES ('{name}', {growthTime})";
            dbCmd.ExecuteNonQuery();
        }
    }

    public void QueryUsers()
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            dbCmd.CommandText = "SELECT * FROM Users";
            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Debug.Log($"UserID: {reader["UserID"]}, Username: {reader["Username"]}, Role: {reader["AccessLevel"]}");
                }
            }
        }
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
}
