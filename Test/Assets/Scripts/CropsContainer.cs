using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/Crops Container")]
public class CropsContainer : ScriptableObject
{
	public List<CropTile> crops;
	public CropTile Get(Vector3Int pos)
	{
		return crops.Find(x => x.position == pos);

	}

	public void Add(CropTile crop)
	{
		crops.Add(crop);
	}
}