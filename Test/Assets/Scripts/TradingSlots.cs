using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TradingSlot// : MonoBehaviour
{
	//[SerializeField] public


	//[SerializeField] int Callitemid;
	// Insert variables ?

	public int amount, buyp, sellp;
	public string Itname= "Default name";
	public Sprite icon;
	//[SerializeField] 
	
	
	
	
}

public class TradingSlots :  MonoBehaviour
{
	public TradingSlot slot;
	//public TradeSlotsContainer container;
	[SerializeField] Image ItemImg;
	[SerializeField] TMP_Text ItemName;
	[SerializeField] TMP_Text AmountItems;
	[SerializeField] TMP_Text Buyprice;
	[SerializeField] TMP_Text SellPrice;
	int callerId;
	
	TradingP ltp;



	public void SetupItemSlots(string Callertag,int i)
	{
		if (slot == null)
			slot = new TradingSlot();
		callerId = i;

		ltp  = GetComponentInParent<TradingP>();

		if (ltp == null)
		{
			Debug.LogError("tradingslots ltp null");
			return;
		}
		if (ltp != null)
		{
			Debug.LogError("tradingslots ltp found ");			
		}

		DatabaseManager.instance.PopulateTradeFields(slot, Callertag, i+1);
		//ItemImg.sprite = icon;
		ItemName.text = "Name: " + slot.Itname;
		AmountItems.text = "Amount: " + slot.amount.ToString();
		Buyprice.text = "Buy - " + slot.buyp.ToString();
		SellPrice.text = "Sell - " + slot.sellp.ToString();
		//slots[id]= id;
		//ItemName.text = "Name: " + container.slots[id].Itname;
		//AmountItems.text = "Amount: " + container.slots[id].amount.ToString();
		//Buyprice.text = "Buy - " + container.slots[id].buyp.ToString();
		//SellPrice.text = "Sell - " + container.slots[id].sellp.ToString();

		//if (ltp == null)
		//{
		//	Debug.LogError("ltp in Tradslots null");
		//	return;
		//}
	}

	

	public void BuyItems()
	{

		ltp.BuyItems(slot.amount,callerId);

		//Debug.Log("Bought " + AmountItems.text);
	}

	public void SellItems()
	{
		ltp.SellItems(slot.amount, callerId);
		//Debug.Log("Bought " + AmountItems.text);
	}
}
