
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

[Serializable]
public class CropTile
{
    public int growTimer;
    public Crop crop;
    public int growStage;
    public SpriteRenderer renderer;
    public Vector3Int position;
    public int damage;// used to wither plants 
    public bool Completed
    {
        get 
        {
            if (crop == null) { return false; }
            return growTimer >= crop.timeToGrow;
        }
    }

	internal void Harvested()
	{
        growTimer = 0;
        growStage = 0;
        crop = null;
        renderer.gameObject.SetActive(false);
        damage = 0;

	}
}
public class CropManager : MonoBehaviour
{

    public TilemapCropsManager cropsManager;

    public void PickUp(Vector3Int pos)
    {
        if (cropsManager == null)
        {
            Debug.LogWarning("No tilemap crops manager are referenced in the crops manager");
            return;
        }
        cropsManager.PickUp(pos);
    }

    public bool CheckTile(Vector3Int pos)
    {
		if (cropsManager == null)
		{
			Debug.LogWarning("No tilemap crops manager are referenced in the crops manager");
			return false;
		}
		return cropsManager.CheckTile(pos);
    }

	public void Seed(Vector3Int pos, Crop toSeed)
	{
		if (cropsManager == null)
		{
			Debug.LogWarning("No tilemap crops manager are referenced in the crops manager");
			return;
		}

        cropsManager.Seed(pos, toSeed);
	}

	public void Plow(Vector3Int pos)
	{
		if (cropsManager == null)
		{
			Debug.LogWarning("No tilemap crops manager are referenced in the crops manager");
			return;
		}
		cropsManager.Plow(pos);
	}
}
