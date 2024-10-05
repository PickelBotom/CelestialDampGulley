using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="Date/Tool Action/Plow")]

public class PlowTile : ToolAction
{
    [SerializeField]List<TileBase> canPlow;
    public override bool OnApplyToTileMap(Vector3Int gridPosition, TileMapReadController tileMapReadController)
    {
        TileBase tileToPlow = tileMapReadController.GetTileBase(gridPosition);
        if(canPlow.Contains(tileToPlow)==false){
            return false;
        }

        tileMapReadController.cropManager.Plow(gridPosition);
        return true;
    }
}
