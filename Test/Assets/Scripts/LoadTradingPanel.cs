using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Cinemachine.CinemachineOrbitalTransposer;
using static UnityEditor.Progress;

public class LoadTradingPanel : MonoBehaviour
{
	//   TradingSlots ts1;
	//TradingSlots ts2;
	//TradingSlots ts3;
	[SerializeField] GameObject TradingPanel;
	TradingSlots[] TradSlots;
	
	string callertag;
	
	public string CallerTag { set { callertag = value; } } 

	[SerializeField] GameObject Inventory;
	TradeInteractable store;
	
	bool isActive;
	//private void Start()
	//{
		
	//	//gameObject.SetActive(true);
		
	//	//TradSlots = GetComponentsInChildren<TradingSlots>();

	//	//foreach (TradingSlots slot in TradSlots)
	//	//{
	//	//	DatabaseManager.instance.PopulateTradeFields(slot, Callertag);// maybe set variable to change and then assign ?
	//	//	slot.SetupItemSlots();
	//	//}

	//}
	public void BeginTrading(TradeInteractable store) 
	{
		this.store = store;

		Debug.LogError("Begin trade");
		loadTradeItems();
		ToggleTradingPanel();

	}

		public void loadTradeItems()
	{
		int itemid=0;
		TradSlots = TradingPanel.GetComponents<TradingSlots>();
		if (TradSlots == null)
		{
			Debug.Log("TradSlo0ts null");
			return;
		}
		foreach (TradingSlots slot in TradSlots)
		{
			DatabaseManager.instance.PopulateTradeFields(slot, callertag);// maybe set variable to change and then assign ?
			slot.SetupItemSlots(itemid);
			itemid++;
		}
		ToggleTradingPanel();
	}
	public void ToggleTradingPanel()
	{ 
		isActive = TradingPanel.activeInHierarchy;
		if (isActive)
			TradingPanel.SetActive(false);
		else
			TradingPanel.SetActive(true);
	}

	public void BuyItems(int amount,int itemid)
	{
		Item it= store.TradeItems[itemid];
		// interact with inventory
		Debug.Log("Bought " + it.Name+" "+amount );
	}

	public void SellItems(int amount, int itemid)
	{
		Item it = store.TradeItems[itemid];
		// interact with inventory
		Debug.Log("Sold " + it.Name + " " + amount);
	}
}
