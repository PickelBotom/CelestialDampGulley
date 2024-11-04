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
        //UpdatePlayerGoldUI();


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

            tradeSlots[i].SetupItemSlots(store.TradeItems[i], i);



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
        Debug.Log("Bought " + item.Name + " x" + amount);
    }

    public void SellItems(int amount, int itemId)
{
    Item item = store.TradeItems[itemId];
    InventoryPanel inventoryPanel = Inventory.GetComponent<InventoryPanel>();


    int amountToSell = amount;
    if (inventoryPanel != null && inventoryPanel.inventory.HasItem(item, amountToSell))
    {
        int itemValue = item.SellPrice * amountToSell;
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
