using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	[SerializeField] GameObject TutPanel;
	[SerializeField] TMP_InputField tutText;

	public void LoadTutfromID(int tutID)
	{
		tutText.text = DatabaseManager.instance.PopulateTutfield(tutID);
		TutPanel.SetActive(true);
	}
	public bool CheckIsActive()
	{
		return TutPanel.activeInHierarchy;	
	}
}
