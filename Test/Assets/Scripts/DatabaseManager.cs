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

        // Create tables if not exist
        CreateTable();

        // Example usage
        AddUser("admin", "admin123", "Admin");
        AddCrop("Wheat", 30);

        // Close the connection when done
        dbConnection.Close();
    }

    private void CreateTable()
    {
        using (IDbCommand dbCmd = dbConnection.CreateCommand())
        {
            // Create Users table
            dbCmd.CommandText = "CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT, Password TEXT, Role TEXT)";
            dbCmd.ExecuteNonQuery();

            // Create Crops table
            dbCmd.CommandText = "CREATE TABLE IF NOT EXISTS Crops (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, GrowthTime INTEGER)";
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
                    Debug.Log($"User ID: {reader["Id"]}, Username: {reader["Username"]}, Role: {reader["Role"]}");
                }
            }
        }
    }
}
