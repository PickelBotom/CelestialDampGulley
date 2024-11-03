using System.Collections.Generic;
using UnityEngine;

public class TradeInteractable : Interactable
{
    [SerializeField] public List<Item> TradeItems; // List of items to trade
    [SerializeField] private List<int> SellAmounts; // List of amounts to sell for each item

    [SerializeField] private string Callertag;

    public override void Interact(Character character)
    {
        if (character == null)
        {
            Debug.LogError("Character is null!");
            return;
        }
        
        LoadTradingPanel trading = character.GetComponent<LoadTradingPanel>();
        Debug.LogError("Trade interact");

        if (trading == null) 
        {
            Debug.Log("Trading null");
            return;
        }

        trading.CallerTag = Callertag;
        trading.BeginTrading(this);
    }

    // Method to get the sell amount for a specific item index
    public int GetSellAmount(int index)
    {
        if (index >= 0 && index < SellAmounts.Count)
        {
            return SellAmounts[index];
        }
        return 0; // Return 0 if index is out of range
    }

    // Method to get the items for trading
    public List<Item> GetTradeItems()
    {
        return TradeItems;
    }
}
