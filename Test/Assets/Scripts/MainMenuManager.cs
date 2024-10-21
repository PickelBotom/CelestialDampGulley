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

	private	GameObject tempobj;

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
	[SerializeField] GameObject ADDUserPanel;
	[SerializeField] TMP_InputField ADDUsername;
	[SerializeField] TMP_InputField ADDPassword;
	[SerializeField] TMP_Dropdown ADDRoleDp;
	int ADDRoleEntry;
	string ADDRoleName;

	[Header("ManageUser")]
	[SerializeField] GameObject ManageUserPanel;
	[SerializeField] TMP_InputField MUUsername;
	[SerializeField] TMP_Dropdown MURoleDp;
	[SerializeField] TextMeshProUGUI MUScrollviewText;
	int MURoleEntry;
	string MURoleName;

	[Header("Notifications")]
	[SerializeField] GameObject NotificationPanel;
	[SerializeField] TMP_InputField NotifText;

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
		tempobj = Loginpanel;
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
		NotificationPanel.SetActive(true);
		NotifText.text = message;
		tempobj.SetActive(false);

		Debug.Log(message); 
	}
	public void RestorePanel()
	{
		NotificationPanel.SetActive(false);
		tempobj.SetActive(true);
	}

	public void AddUser()
	{
		 username = ADDUsername.text;
		 password = ADDPassword.text;
		 tempobj = ADDUserPanel;
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
		username = MUUsername.text;
		tempobj = ManageUserPanel;
		if (FindUser(username))
		{
			MURoleEntry = MURoleDp.value;
			MURoleName = MURoleDp.options[MURoleEntry].text;
			using (IDbCommand dbCmd = dbConnection.CreateCommand())
			{
				dbCmd.CommandText = $"Update Users SET Role = '{MURoleName}' WHERE Username = '{username}'";
				dbCmd.ExecuteNonQuery();
			}
			RefreshList();
			MUUsername.text = "";
			OutputMessage(username + " Role Successfully changed!");
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
	public void RefreshList()
	{
		string tuser, trole;
		MUScrollviewText.text = "Username\tRole\n";
		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			dbCmd.CommandText = $"SELECT Username,Role FROM Users WHERE Role != '{"Admin"}'";
			using (IDataReader reader = dbCmd.ExecuteReader())
			{
				while (reader.Read())
				{
					tuser = reader["Username"].ToString();
					trole = reader["Role"].ToString();
					MUScrollviewText.text += tuser + "\t"+trole+ "\n";
					
				}

			}

		}
	}
	public void DeleteUser()
	{
		username = MUUsername.text;
		tempobj = ManageUserPanel;
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
