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
	//TradSlots;



	public string CallerTag;

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
		//ToggleTradingPanel();

	}

		public void loadTradeItems()
	{
		//TradingSlots slot;
		//int itemid=0;
		TradingSlots[] TradSlots = TradingPanel.GetComponentsInChildren<TradingSlots>();
		ToggleTradingPanel();

		if (TradSlots == null)
		{
			Debug.LogError("TradSlo0ts not null");
			//Debug.LogError(TradSlots[0].name);
			return;
		}
		if (TradSlots == null)
		{
			Debug.LogError(TradSlots[0].name);
		}
		Debug.LogError("Slots count :"+ TradSlots.Length);

		for (int i = 0; i < TradSlots.Length; i++)
		{
			//DatabaseManager.instance.PopulateTradeFields(TradSlots[i].slot, callertag);
			Debug.LogError("CallerTag :" + CallerTag);
			TradSlots[i].SetupItemSlots(CallerTag,i);
			Debug.LogError("Slot Amount"+TradSlots[i].slot.amount);

			
		}

		//foreach (TradingSlots slot in TradSlots)
		//{
		//	DatabaseManager.instance.PopulateTradeFields(slot, callertag);// maybe set variable to change and then assign ?
		//	slot.SetupItemSlots(itemid);
		//	itemid++;
		//	Debug.LogError(slot.amount);
		//}
		Debug.LogError("Reaches here");
		
	}
	public void ToggleTradingPanel()
	{
	
		isActive = TradingPanel.activeInHierarchy;
		if (isActive)
		{ TradingPanel.SetActive(false);

		}
		else
		{	
			TradingPanel.SetActive(true);
		}
			
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
