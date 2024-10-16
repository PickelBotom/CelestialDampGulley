using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu( menuName = "Data/Crop")]
public class Crop : Item
{
public int timeToGrow=10;
public Item yield;
public int count = 1;

public List<Sprite> sprites;
public List<int> growthStageTime;
public bool Stackable;
}


