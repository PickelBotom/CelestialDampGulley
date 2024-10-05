using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/ToolAction/Seed Tile")]

public class SeedTile : ToolAction
{
    // Start is called before the first frame update
    public override bool OnApplyToTileMap(Vector3Int gridPosition, TileMapReadController tileMapReadController)
    {

        if(tileMapReadController.cropManager.CheckTile(gridPosition) == false){
            return false;
        }

        tileMapReadController.cropManager.Seed(gridPosition);

        return true;
    }

    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        inventory.RemoveItem(usedItem);
    }
}
