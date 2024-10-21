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

public class MainMenuManager : MonoBehaviour
{


	string inputpassword;
	string inputusername;
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

	[Header("ManageUser")]
	[SerializeField] TMP_InputField MUUsername;

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
		if (!FindUser(LogUsername.text))
		{
			return;
		}

		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			dbCmd.CommandText = $"SELECT Password,Role FROM Users WHERE Username = '{LogUsername.text}'";
			using (IDataReader reader = dbCmd.ExecuteReader())
			{
				if (LogPassword.text == reader["Password"].ToString())
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

	}

	public void ManageUser()
	{

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
		OutputMessage(usern + "not found");
		return false;

	}

}
