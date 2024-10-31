using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeInteractable : Interactable
{
	// insert items here ?

	public List<Item> TradeItems;
	[SerializeField] string Callertag;
	public override void Interact(Character character)
	{
		if (character == null)
		{
			Debug.LogError("Character is null!");
			return;
		}
		LoadTradingPanel trading = character.GetComponent<LoadTradingPanel>();
		Debug.LogError("Trade interact");
		if (trading == null) {
			Debug.Log("Trading null");
			return; }
		trading.CallerTag = Callertag;
		trading.BeginTrading(this);
	}

}
