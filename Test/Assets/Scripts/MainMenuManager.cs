using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using TMPro;
using System.IO;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	string password;
	string username;
	int userid = 0;

	private string dbPath;
	private IDbConnection dbConnection;

	private GameObject tempobj;

	[Header("MainMenu")]
	[SerializeField] GameObject MainMenuPanel;
	[SerializeField] GameObject BackgroundPNG;

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
	[SerializeField] TMP_InputField MUPassword;
	[SerializeField] TMP_Dropdown MURoleDp;
	[SerializeField] TextMeshProUGUI MUScrollviewText;
	int MURoleEntry;
	string MURoleName;


	[Header("Notifications")]
	[SerializeField] GameObject NotificationPanel;
	[SerializeField] TMP_InputField NotifText;

	[Header("Settings")]
	[SerializeField] GameObject SettingsPanel;
	

	private void Start()
	{
		MainMenuPanel.SetActive(true);
		BackgroundPNG.SetActive(true);
		Loginpanel.SetActive(false);
		AdminPanel.SetActive(false);

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
		username = LogUsername.text;
		password = LogPassword.text;
		tempobj = Loginpanel;

		if (!FindUser(username))
		{
			OutputMessage(username + " not found");
			return;
		}

		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			dbCmd.CommandText = $"SELECT Password, UserID FROM Users WHERE Username = '{username}'";
			using (IDataReader reader = dbCmd.ExecuteReader())
			{
				if (reader.Read())
				{
					string storedHash = reader["Password"].ToString();
					if (storedHash == HashPassword(password))
					{
						userid = int.Parse(reader["UserID"].ToString());
						LogUsername.text = "";
						LogPassword.text = "";

						if (getRole(userid) != "Admin")
							StartGame();
						else
							LoadAdminMenu();
					}
					else
					{
						OutputMessage("Invalid password");
					}
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
		GameManager.userid = userid;
		Debug.LogError(GameManager.userid);

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

	public void RestoreSettingPaneltoPrevious()
	{
		SettingsPanel.SetActive(false);
		tempobj.SetActive(true);
	}

	public void SetpreviousMMPanel()
	{
		tempobj = MainMenuPanel;
		SettingsPanel.SetActive(true);
		tempobj.SetActive(false);
	}
	public void SetpreviousMMAPanel()
	{
		tempobj = AdminPanel;
		SettingsPanel.SetActive(true);
		tempobj.SetActive(false);


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
		if (string.IsNullOrEmpty(username))
		{
			OutputMessage("Enter a username");
			return;
		}
		if (string.IsNullOrEmpty(password))
		{
			OutputMessage("Enter a password");
			return;
		}

		if(!checkValidPassword(password))
			return;

		ADDRoleEntry = ADDRoleDp.value+1;
		//ADDRoleName = ADDRoleDp.options[ADDRoleEntry].text;

		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			string hashedPassword = HashPassword(password);
			dbCmd.CommandText = $"INSERT INTO Users (Username, Password, RoleID) VALUES ('{username}', '{hashedPassword}', '{ADDRoleEntry}')";
			dbCmd.ExecuteNonQuery();
			Debug.Log($"Hashed password for '{username}': {hashedPassword}");
		}

		ADDUsername.text = "";
		ADDPassword.text = "";
		OutputMessage("User Successfully Added :)");
		Debug.Log("{hashedPassword}");
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

	public void ConfirmChanges()
	{
		username = MUUsername.text;
		password = MUPassword.text;
		tempobj = ManageUserPanel;
		if (FindUser(username))
		{
			if (!string.IsNullOrEmpty(password))
			{
				if (!checkValidPassword(password))
					return;
				string hashedPassword = HashPassword(password);
				using (IDbCommand dbCmd = dbConnection.CreateCommand())
				{
					dbCmd.CommandText = $"Update Users SET Password = '{hashedPassword}' WHERE Username = '{username}'";
					dbCmd.ExecuteNonQuery();
				}
			}
			MURoleEntry = MURoleDp.value+1;
			//MURoleName = MURoleDp.options[MURoleEntry].text;
			using (IDbCommand dbCmd = dbConnection.CreateCommand())
			{
				dbCmd.CommandText = $"Update Users SET RoleID = '{MURoleEntry}' WHERE Username = '{username}'";
				dbCmd.ExecuteNonQuery();
			}
			RefreshList();
			MUUsername.text = "";
			MUPassword.text = "";
			OutputMessage(username + " Successfully Updated!");
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



public bool checkValidPassword(string pass) // must still add to ADDUSER and in future add to EDITPASSWORD
	{
		bool validpass = true;
		string issues = "";

		if (pass.Length < 6)
		{ 
			validpass= false;
			issues += "Password must be more than 6 letters.\n";
		}
		char[] specialChar = "!@#$%^&*:?".ToCharArray();
		bool specialchar =false;
		foreach (char c in specialChar)
		{
			if (pass.Contains(c))
			{
				specialchar = true;				
			}
		}
		if (!specialchar)
		{
			validpass = false;
			issues += "Password must contain a special character. ";

		}
		if (issues.Length != 0)
			OutputMessage(issues);

		return validpass;
	}


	public void RefreshList()
	{
		string tuser, trole;
		MUScrollviewText.text = "Username\tRole\n";
		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			dbCmd.CommandText = @"
					  SELECT Users.Username, Roles.UserType
					  FROM Users
					  JOIN Roles ON Users.RoleID = Roles.RoleID
					  WHERE Roles.UserType <> 'Admin'";

			//dbCmd.CommandText = $"SELECT Username, RoleID FROM Users WHERE Role != 'Admin'";
			using (IDataReader reader = dbCmd.ExecuteReader())
			{
				while (reader.Read())
				{
					tuser = reader["Username"].ToString();
					trole = reader["UserType"].ToString();
					MUScrollviewText.text += tuser + "\t" + trole + "\n";
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
				dbCmd.CommandText = $"SELECT Username, Roles.UserType FROM Users  JOIN Roles ON Users.RoleID = Roles.RoleID WHERE Username = '{username}'";
				using (IDataReader reader = dbCmd.ExecuteReader())
				{
					if (reader.Read() && reader["UserType"].ToString() == "Admin")
					{
						OutputMessage("Cannot Delete Admin");
						return;
					}
				}

				dbCmd.CommandText = $"DELETE FROM Users WHERE Username = '{username}'";
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


	public void SetFullScreen(bool IsfullSc)
	{ 
		Screen.fullScreen = IsfullSc;
		Debug.Log("Fullscreen: "+ IsfullSc);
	}

	// get role of user ID
	public string getRole(int userid)
	{
		string role = "Player";
		using (IDbCommand dbCmd = dbConnection.CreateCommand())
		{
			dbCmd.CommandText = $"SELECT Users.UserID, Roles.UserType" +
				$" FROM Users " +
				$"JOIN Roles ON Users.RoleID = Roles.RoleID " +
				$"WHERE Users.UserID = {userid}";

			using (IDataReader reader = dbCmd.ExecuteReader())
			{
				if (reader.Read())
				{
					role = reader.GetString(1);
				}
				else
				{
					Debug.LogError($" something else went wrong in the pulling of Role Types");
				}
			}
		}
		return role;
	}


}
