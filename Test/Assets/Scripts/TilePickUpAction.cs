using System.Collections;
using UnityEngine;

//namespace Assets.Scripts
//{
[CreateAssetMenu(menuName ="Data/ToolAction/Harvest")]
	public class TilePickUpAction : ToolAction
	{
	public override bool OnApplyToTileMap(Vector3Int gridPosition, TileMapReadController tileMapReadController, Item item)
	{
		tileMapReadController.cropManager.PickUp(gridPosition);
		return true;
	}
}
//}