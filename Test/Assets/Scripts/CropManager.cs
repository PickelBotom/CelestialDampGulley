using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Crops
{


}
public class CropManager : MonoBehaviour
{
    [SerializeField] TileBase plowed;
    [SerializeField] TileBase seeded;
    [SerializeField] Tilemap targetTileMap;

    Dictionary<Vector2Int,Crops> dictcrops;

    private void Start()
    {
        dictcrops = new Dictionary<Vector2Int, Crops>();
    }


    public bool CheckTile(Vector3Int pos)
    {
        return dictcrops.ContainsKey((Vector2Int)pos);
    }

    public void Seed(Vector3Int pos)
    {
        targetTileMap.SetTile(pos,seeded);
    }

    public void Plow(Vector3Int pos )
    {
        if(dictcrops.ContainsKey((Vector2Int)pos))
        {
            return;
        }
        CreatePlowedTile(pos);
    }
    private void CreatePlowedTile(Vector3Int pos)
    {
        Crops crop = new Crops();
        dictcrops.Add((Vector2Int)pos,crop);
        targetTileMap.SetTile(pos,plowed);
    }
}
