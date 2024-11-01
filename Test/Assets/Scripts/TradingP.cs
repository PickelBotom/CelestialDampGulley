using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TradingP : MonoBehaviour
{

	LoadTradingPanel ltp2;
	private void Start()
	{
		ltp2 = GameManager.instance.player.GetComponent<LoadTradingPanel>();
	}


	public void BuyItems(int amount, int itemid)
	{
		ltp2.BuyItems(amount, itemid);
		
	}

	public void SellItems(int amount, int itemid)
	{
		ltp2.SellItems(amount, itemid);
	}
}
