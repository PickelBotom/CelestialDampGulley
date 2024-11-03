using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TradingSlot
{
    public int amount; // This is for selling the item
    public int buyp;
    public int sellp;
    public string Itname = "Default name";
    public Sprite icon;
    
}

public class TradingSlots : MonoBehaviour
{
    public TradingSlot slot;
    [SerializeField] Image ItemImg;
    [SerializeField] TMP_Text ItemName;
    [SerializeField] TMP_Text AmountItems; // Display the amount to sell
    [SerializeField] TMP_Text Buyprice;
    [SerializeField] TMP_Text SellPrice;
    int callerId;

    TradingP ltp;

    public void SetupItemSlots(Item item, int i, int amountForSale) // Add the amount parameter
{
    if (slot == null)
        slot = new TradingSlot();

    callerId = i;
    ltp = GetComponentInParent<TradingP>();

    if (ltp == null)
    {
        Debug.LogError("TradingSlots: Parent TradingP component not found.");
        return;
    }

    // Set slot values from the Item properties
    slot.Itname = item.Name;
    slot.buyp = item.BuyPrice;
    slot.sellp = item.SellPrice;
    slot.icon = item.icon;

    // Set the amount to the one from TradeInteractable
    slot.amount = amountForSale;

    // Update UI elements based on slot values
    ItemName.text = "Name: " + slot.Itname;
    AmountItems.text = "Amount: " + slot.amount.ToString(); // Display the amount
    Buyprice.text = "Buy - " + (slot.buyp * slot.amount).ToString();
    SellPrice.text = "Sell - " + (slot.sellp * slot.amount).ToString(); 
    ItemImg.sprite = slot.icon;
}


    public void UpdateAmount(int newAmount)
    {
        slot.amount = newAmount; // Update the amount based on UI input
        AmountItems.text = "Amount: " + slot.amount.ToString(); // Update the display
    }

    public void BuyItems()
    {
        ltp.BuyItems(slot.amount, callerId);
    }

    public void SellItems()
    {
        ltp.SellItems(slot.amount, callerId);
    }
}
