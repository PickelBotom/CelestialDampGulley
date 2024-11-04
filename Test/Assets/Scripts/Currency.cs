using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Currency : MonoBehaviour
{
	public int gold;
	[SerializeField] TMP_Text GoldText;


	public void UpdateGold()
	{
		GoldText.text = gold + " Gold";
	}

}
