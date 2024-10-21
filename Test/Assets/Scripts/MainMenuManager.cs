using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using Unity.UI;
using TMPro;
using System.IO;
using System;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{


	string password;
	string username;
	string role="";

	private string dbPath;
	private IDbConnection dbConnection;

	[Header("Transition")]
	[SerializeField] string nameEssentialScene;
	[SerializeField] string nameMainScene;


	[Header("Login")]
	[SerializeField] TMP_InputField LogUsername;
	[SerializeField] TMP_InputField LogPassword;

	[Header("AdminMenu")]
	[SerializeField] GameObject AdminPanel;
	[SerializeField] GameObject Loginpanel;


	[Header("AddUser")]
	[SerializeField] TMP_InputField ADDUsername;
	[SerializeField] TMP_InputField ADDPassword;
	[SerializeField] TMP_Dropdown ADDRoleDp;
	int ADDRoleEntry;
	string ADDRoleName;

	[Header("ManageUser")]
	[SerializeField] TMP_InputField MUUsername;
	[SerializeField] TMP_Dropdown MURoleDp;
	[SerializeField] ScrollView MUScrollview;

	private void Start()
	{
		dbPath = Path.Combine(Application.persistentDataPath, "gameDatabase.db");
		dbConnection = new SqliteConnection("URI=file:" + dbPath);
		dbConnection.Open();
	}
	public void ExitGame()
	{
		Debug.Log("Quitting game");
		Application.Quit();
	}

	public void Login()
	{
		username=LogUsername.text;
		password=LogPassword.text;
		if (!FindUser(username))
		{
			OutputMessage(username + " not found");
			return;
		}

		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			dbCmd.CommandText = $"SELECT Password,Role FROM Users WHERE Username = '{username}'";
			using (IDataReader reader = dbCmd.ExecuteReader())
			{
				if (password == reader["Password"].ToString())
				{
					role = reader["Role"].ToString();

					LogUsername.text = "";
					LogPassword.text = "";// for clearing textfield data

					if (role != "Admin")
						StartGame();
					else
						LoadAdminMenu();
				}
				else
				{
					OutputMessage("Invalid password");
					return;
				}

			}
		}

	}

	private void LoadAdminMenu()
	{
		Loginpanel.SetActive(false);
		AdminPanel.SetActive(true);

	}
	public void StartGame()
	{
		GameManager.userRole = role;
		dbConnection.Close();
		SceneManager.LoadScene(nameMainScene, LoadSceneMode.Single);
		SceneManager.LoadScene(nameEssentialScene, LoadSceneMode.Additive);
	}

	void OutputMessage(string message)
	{
		Debug.Log(message); 
	}

	public void AddUser()
	{
		 username = ADDUsername.text;
		 password = ADDPassword.text;
		if (FindUser(username))
		{
			OutputMessage("Username Exists");
			return;
		}
		if(username =="")
		{
			return;
		}

		if (password == "")
		{
			OutputMessage("Enter a password");
			return;
		}
		
		ADDRoleEntry = ADDRoleDp.value;
		ADDRoleName = ADDRoleDp.options[ADDRoleEntry].text;

		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			dbCmd.CommandText = $"INSERT INTO Users (Username, Password, Role) VALUES ('{username}', '{password}', '{ADDRoleName}')";
			dbCmd.ExecuteNonQuery();
		}

		ADDUsername.text = "";
		ADDPassword.text = "";
		
		OutputMessage("User Successfully ADDed :)");


	}

	public void ConfirmChanges()
	{

		OutputMessage(username + " Role Successfully changed!");
	}
	public void RefreshList()
	{
		OutputMessage("List refreshed!");
	}
	public void DeleteUser()
	{
		username = MUUsername.text;

		if (FindUser(username))
		{
			using (IDbCommand dbCmd = dbConnection.CreateCommand())
			{
				dbCmd.CommandText = $"SELECT Username,Role FROM Users WHERE Username = '{username}' ";
				using (IDataReader reader = dbCmd.ExecuteReader())
				{
					if (reader["Role"].ToString() == "Admin")
					{
						OutputMessage("Cannot Delete Admin");
						return;
					}

				}

				dbCmd.CommandText = $"Delete FROM Users WHERE Username ='{username}'";
				dbCmd.ExecuteNonQuery();

				MUUsername.text = "";
				RefreshList();

				OutputMessage(username + " Deleted");
			}
		}
		else
		{
			if (username.Length > 0)
			{
				OutputMessage(username + " not found");
				return;
			}
		}
		
			

		
		
	}

	bool FindUser(string usern)
	{
		if (usern == "")
		{
			OutputMessage("Enter a username!");
			return false;
		}

		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			dbCmd.CommandText = $"SELECT Username FROM Users WHERE Username = '{usern}'";
			using (IDataReader reader = dbCmd.ExecuteReader())
			{
				if (reader.Read())
				{
					return true;
				}

			}

		}
		return false;

	}

}
