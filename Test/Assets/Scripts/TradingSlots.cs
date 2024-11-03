using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] TMP_Text AmountItems;
    [SerializeField] TMP_Text Buyprice;
    [SerializeField] TMP_Text SellPrice;
    int callerId;

    TradingP ltp;

    public void SetupItemSlots(Item item, int i)
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

        // Set amount for selling
        if (i == 0) slot.amount = 1; // First slot sells 1
        else if (i == 1) slot.amount = 5; // Second slot sells 5
        else if (i == 2) slot.amount = 10; // Third slot sells 10

        // Update UI elements based on slot values
        ItemName.text = "Name: " + slot.Itname;
        AmountItems.text = "Amount: " + slot.amount.ToString();
        Buyprice.text = "Buy - " + slot.buyp.ToString();
        SellPrice.text = "Sell - " + slot.sellp.ToString();
        ItemImg.sprite = slot.icon;
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
