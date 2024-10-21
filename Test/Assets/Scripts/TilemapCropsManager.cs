
using UnityEngine;
using UnityEngine.Tilemaps;



public class TilemapCropsManager : TimeAgent
{
		[SerializeField] CropsContainer container;

		[SerializeField] TileBase plowed;
		[SerializeField] TileBase seeded;

		//[SerializeField]
		Tilemap targetTileMap;

		[SerializeField] GameObject cropsSpritePrefab;


		private void Start()
		{
		GameManager.instance.GetComponent<CropManager>().cropsManager = this;// i have no idea what he said here 
		// min 25 ish talking about how it was inefficient before  and now he makes it better some how ?
			targetTileMap = GetComponent<Tilemap>();
			onTimeTick += Tick;
			Init();
			VisualizeMap();
		}

	private void VisualizeMap()
	{
		for (int i = 0; i < container.crops.Count; i++)
		{
			VisualizeTile(container.crops[i]);
		}
	}

	public void Tick()
		{
			
			foreach (CropTile cropTile in container.crops)
			{
				if (cropTile.crop == null) { continue; }

				cropTile.damage += 1;
				if (cropTile.damage > (cropTile.crop.timeToGrow + 5)) // dies five ticks after fully grown 
				{
					cropTile.Harvested();
					targetTileMap.SetTile(cropTile.position, plowed);
					continue;// added to try fix empty croptile renderer
				}


				if (cropTile.Completed)
				{ continue; }

				cropTile.growTimer += 1;

				if (cropTile.growTimer >= cropTile.crop.growthStageTime[cropTile.growStage])// need to fix
				{
					Debug.Log("Tick for crop");
					cropTile.renderer.gameObject.SetActive(true);
					cropTile.renderer.sprite = cropTile.crop.sprites[cropTile.growStage];

					cropTile.growStage += 1;
				}


			}
		}

		public bool CheckTile(Vector3Int pos)
		{

			return container.Get(pos) !=null;
		}

		public void Seed(Vector3Int pos, Crop toSeed)
		{
			CropTile tile = container.Get(pos);

			if (tile == null)
			{ return; }

			targetTileMap.SetTile(pos, seeded);

			tile.crop = toSeed;
		}

		public void Plow(Vector3Int pos)
		{
		if (CheckTile(pos) == true) { return; }
			CreatePlowedTile(pos);
		}
		private void CreatePlowedTile(Vector3Int pos)
		{
		

			CropTile crop = new CropTile();
			container.Add( crop);

			targetTileMap.SetTile(pos, plowed);

			

			crop.position = pos;
			VisualizeTile(crop);

	}

		internal void PickUp(Vector3Int gridPosition)
		{

			Vector2Int pos = (Vector2Int)gridPosition;

			CropTile tile = container.Get(gridPosition);
			if (tile == null) { return; }

			if (tile.Completed)
			{
				ItemSpawnManager.instance.SpawnItem(targetTileMap.CellToWorld(gridPosition),
					tile.crop.yield,
					tile.crop.count);

				
				tile.Harvested();
			VisualizeTile(tile);
			}
		}

	private void OnDestroy()
	{
		for (int i = 0; i < container.crops.Count; i++)
		{
			container.crops[i].renderer = null;
		}
	}
	public void VisualizeTile(CropTile cropTile)
	{
		targetTileMap.SetTile(cropTile.position, cropTile.crop != null ? seeded : plowed);

		if (cropTile.renderer == null)
		{
			GameObject go = Instantiate(cropsSpritePrefab,transform);
			go.transform.position = targetTileMap.CellToWorld(cropTile.position);
			
			cropTile.renderer = go.GetComponent<SpriteRenderer>();
		}

		bool growing = cropTile.crop != null && cropTile.growTimer >= cropTile.crop.growthStageTime[0];
		cropTile.renderer.gameObject.SetActive(growing);
		if (growing)
		{
			cropTile.renderer.sprite = cropTile.crop.sprites[cropTile.growStage-1];
		}
		
	}
}