using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Trade/Trades")]
public class TradeSlotsContainer : ScriptableObject
{
	public List<TradingSlot> slots;
}
