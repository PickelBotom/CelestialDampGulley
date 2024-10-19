using UnityEngine;

public class ItemToolbarPanel : ItemPanel
{
    [SerializeField] ToolBarController toolbarController;

    void Start()
    {
        Init();
        toolbarController.onChange += Highlight;
        Highlight(0);
    }
    
    public override void OnClick(int id)
    {
        Debug.Log($"Button clicked at index: {id}");
        toolbarController.Set(id);  // Update selected tool
        Highlight(id);              // Highlight the clicked item
        RefreshItemDisplay();
    }

    int currentSelectedTool;
    public void Highlight(int id)
    {
        buttons[currentSelectedTool].Highlight(false);  // Clear previous highlight
        currentSelectedTool = id;                       // Update current selected tool
        buttons[currentSelectedTool].Highlight(true);    // Highlight the new selected tool
    }

    private void RefreshItemDisplay()
    {
        Item selectedItem = toolbarController.GetItem;
        // Update the UI components to display the selected item details
        Debug.Log($"Selected Item: {selectedItem?.Name}");
    }
}

