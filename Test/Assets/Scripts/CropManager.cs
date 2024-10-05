using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CropTile
{
    public int growTimer;
    public Crop crop;
    public int growStage;
    public SpriteRenderer renderer;
}
public class CropManager : TimeAgent
{
    [SerializeField] TileBase plowed;
    [SerializeField] TileBase seeded;
    [SerializeField] Tilemap targetTileMap;

    [SerializeField] GameObject cropsSpritePrefab;

    Dictionary<Vector2Int,CropTile> dictcrops;

    private void Start()
    {
        dictcrops = new Dictionary<Vector2Int, CropTile>();
        onTimeTick+= Tick;
        Init();
    }

    public void Tick()
    {
        foreach(CropTile cropTile in dictcrops.Values)
        {
            if (cropTile.crop == null){continue;}
            
            cropTile.growTimer +=1;
            
            if (cropTile.growTimer>= cropTile.crop.growthStageTime[cropTile.growStage])
            {  
                Debug.Log("Tick for crop");
                cropTile.renderer.gameObject.SetActive(true);
                cropTile.renderer.sprite= cropTile.crop.sprites[cropTile.growStage];

                cropTile.growStage+=1;
            }

            if (cropTile.growTimer>= cropTile.crop.timeToGrow)
            {
                cropTile.crop = null;
            }
        }
    }

    public bool CheckTile(Vector3Int pos)
    {
        return dictcrops.ContainsKey((Vector2Int)pos);
    }

    public void Seed(Vector3Int pos, Crop toSeed)
    {
        targetTileMap.SetTile(pos,seeded);

        dictcrops[(Vector2Int)pos].crop = toSeed;
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
        CropTile crop = new CropTile();
        dictcrops.Add((Vector2Int)pos,crop);
        targetTileMap.SetTile(pos,plowed);
        
        GameObject go = Instantiate(cropsSpritePrefab);
        go.transform.position = targetTileMap.CellToWorld(pos);
        go.SetActive(false);
        crop.renderer = go.GetComponent<SpriteRenderer>();
    }
}
