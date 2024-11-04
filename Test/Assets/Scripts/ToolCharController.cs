using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolCharController : MonoBehaviour
{
    PlayerController player;
    Rigidbody2D rb2d;  
    Animator animator;

    [SerializeField] float offsetDistance = 0.11f;
    [SerializeField] float sizeOFInteractArea = 0.2f;
    [SerializeField] MarkerManager markerManager;
    [SerializeField] TileMapReadController tileMapReadController;
    [SerializeField] float maxDistance = 0.5f;
    [SerializeField] ToolAction onTilePickUp; 
    [SerializeField] EnvironmentManager environmentManager; // Add this reference

    [SerializeField] ToolBarController toolBarController;

    Vector3Int selectedTilePosition;
    bool selectable;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        SelectTile();
        CanSelectCheck();
        Marker();

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right click pressed");
            if (UseToolWorld())
            { return; }
            UseToolGrid();
        }
    }

    private void SelectTile()
    {
        selectedTilePosition = tileMapReadController.GetGridPosition(Input.mousePosition, true);
    }

    void CanSelectCheck()
    {
        Vector2 charPos = transform.position;
        Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectable = Vector2.Distance(charPos, cameraPos) < maxDistance;
        markerManager.Show(selectable);
    }

    private void Marker()
    {
        markerManager.markedCellPosition = selectedTilePosition;
    }

    private bool UseToolWorld()
    {
        Vector2 pos = rb2d.position + player.LastMotionVector * offsetDistance;
        Item item = toolBarController.GetItem;

        if (item == null)
            return false;

        if (item.OnAction == null)
            return false;

        animator.SetTrigger("Act");
        bool complete = item.OnAction.OnApply(pos);

        if (complete)
        {
            item.onItemUsed?.OnItemUsed(item, GameManager.instance.inventoryContainer);
        }

        return complete;
    }

    private void UseToolGrid()
    {
        if (selectable == true)
        {
            Item item = toolBarController.GetItem;

            if (item == null)
            {
                PickUpTile();
                return;
            }
            if (item.onTileMapAction == null) { return; }

            animator.SetTrigger("Act");
            bool complete = item.onTileMapAction.OnApplyToTileMap(selectedTilePosition,
                tileMapReadController, item);

            if (complete == true)
            {
                if (item.onItemUsed != null)
                {
                    item.onItemUsed.OnItemUsed(item, GameManager.instance.inventoryContainer);
                }
            }
        }
    }

    private void PickUpTile()
{
    if (onTilePickUp == null)
    { 
        return; 
    }

    onTilePickUp.OnApplyToTileMap(selectedTilePosition, tileMapReadController, null);

    // Get the current item being held in the toolbar
    Item item = toolBarController.GetItem;

    // Check if the item picked up is trash
    if (item != null && item.IsTrash) // Assuming your item has an IsTrash property
    {
        environmentManager.PickUpTrash(); // Call the method to modify environmental health

        // Add the item to the inventory
        GameManager.instance.inventoryContainer.Add(item); // Add the trash item to the player's inventory
    }
}


}
