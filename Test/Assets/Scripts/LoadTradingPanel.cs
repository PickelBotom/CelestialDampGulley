using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadTradingPanel : MonoBehaviour
{
    [SerializeField] GameObject TradingPanel;
    [SerializeField] private GameObject Inventory;
    [SerializeField] private TMP_Text playerGoldText;
    [SerializeField] private Button closeButton;

    private TradeInteractable store;
    private bool isActive;
    private int playerGold = 100;

    public string CallerTag;

    private void Start()
    {
        UpdatePlayerGoldUI();

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseTradingPanel);
        }
    }

    public void BeginTrading(TradeInteractable store)
    {
        this.store = store;
        Debug.Log("Begin trade");
        loadTradeItems();
    }

    public void loadTradeItems()
{
    TradingSlots[] tradeSlots = TradingPanel.GetComponentsInChildren<TradingSlots>();
    ToggleTradingPanel();

    if (tradeSlots == null || tradeSlots.Length == 0)
    {
        Debug.LogError("No trade slots available");
        return;
    }

    for (int i = 0; i < tradeSlots.Length; i++)
    {
        // Get the item from the store
        Item item = store.GetTradeItems()[i];
        int amountForSale = store.GetSellAmount(i); // Get the amount for sale from TradeInteractable

        // Setup item slot with the amount obtained
        tradeSlots[i].SetupItemSlots(item, i, amountForSale); // Add a parameter for the amount
        Debug.Log("Loaded slot " + i + " for trading.");
    }
}


    public void ToggleTradingPanel()
    {
        isActive = TradingPanel.activeInHierarchy;
        TradingPanel.SetActive(!isActive);
    }

    public void CloseTradingPanel()
    {
        TradingPanel.SetActive(false);
    }

    public void BuyItems(int amount, int itemId)
{
    Item item = store.TradeItems[itemId];
    int totalCost = item.BuyPrice * amount; // Calculate total cost

    // Check if player has enough gold
    if (playerGold >= totalCost)
    {
        // Add item to inventory
        InventoryPanel inventoryPanel = Inventory.GetComponent<InventoryPanel>();
        if (inventoryPanel != null)
        {
            inventoryPanel.inventory.Add(item, amount); // Assuming AddItem adds the item to the inventory
            playerGold -= totalCost; // Deduct total cost from player gold
            UpdatePlayerGoldUI(); // Update the UI to reflect new gold amount

            Debug.Log($"Bought {item.Name} x{amount} for {totalCost} gold.");
        }
        else
        {
            Debug.LogWarning("Inventory panel not found.");
        }
    }
    else
    {
        Debug.LogWarning("Not enough gold to buy.");
    }
}

    public void SellItems(int amount, int itemId)
    {
        Item item = store.TradeItems[itemId];
        InventoryPanel inventoryPanel = Inventory.GetComponent<InventoryPanel>();

        int amountToSell = amount;
        if (inventoryPanel != null && inventoryPanel.inventory.HasItem(item, amountToSell))
        {
            int itemValue = item.SellPrice * amountToSell; // Calculate total value
            inventoryPanel.inventory.RemoveItem(item, amountToSell);
            playerGold += itemValue;
            UpdatePlayerGoldUI();

            Debug.Log($"Sold {item.Name} x{amountToSell} for {itemValue} gold.");
        }
        else
        {
            Debug.LogWarning("Not enough items to sell.");
        }
    }

    private void UpdatePlayerGoldUI()
    {
        if (playerGoldText != null)
        {
            playerGoldText.text = "Gold: " + playerGold;
        }
    }
}
