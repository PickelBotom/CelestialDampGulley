using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingSlots : MonoBehaviour
{
	//[SerializeField] public
	[SerializeField] Image ItemImg;
	[SerializeField] TMP_Text ItemName;
	[SerializeField] TMP_Text AmountItems;
	[SerializeField] TMP_Text Buyprice;
	[SerializeField] TMP_Text SellPrice;


	[SerializeField] int Callitemid;
	// Insert variables ?
	public int amount, buyp, sellp;
	public string Itname;
	public Sprite icon;
	LoadTradingPanel ltp;
	public void SetupItemSlots(int id)
	{
		LoadTradingPanel ltp = gameObject.GetComponentInParent<LoadTradingPanel>();
		ItemImg.sprite = icon;
		ItemName.text = "Name: "+Itname;
		AmountItems.text = "Amount: "+amount.ToString();
		Buyprice.text = "Buy - "+buyp.ToString();
		SellPrice.text = "Sell - "+sellp.ToString();
		Callitemid = id;
	}
	public void BuyItems()
	{

		ltp.BuyItems(amount,Callitemid);

		Debug.Log("Bought " + AmountItems.text);
	}

	public void SellItems()
	{
		ltp.SellItems(amount,Callitemid);
		Debug.Log("Bought " + AmountItems.text);
	}
}
