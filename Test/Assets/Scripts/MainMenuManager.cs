using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    [SerializeField] string nameEssentialScene;
	[SerializeField] string nameMainScene;

	string inputpassword;
	string inputusername;

	public void ExitGame()
	{
		Debug.Log("Quitting game");
		Application.Quit();
	}

	public void Login()
	{ 
		StartGame();
	}

	public void StartGame()
	{
		SceneManager.LoadScene(nameMainScene, LoadSceneMode.Single);
		SceneManager.LoadScene(nameEssentialScene,LoadSceneMode.Additive);
	}

	public void AddUser()
	{ 
	
	}

	public void ManageUser()
	{ 
	
	}

}
