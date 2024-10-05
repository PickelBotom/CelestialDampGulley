using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TimeAgent))]
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] Item toSpawn;
    [SerializeField] int count;
    [SerializeField] float spread=1f;
    [SerializeField] float probability = 0.15f;
void Start()
{
    TimeAgent tAgent = GetComponent<TimeAgent>();
    tAgent.onTimeTick += Spawn;
}

    void Spawn()
    {
         if(UnityEngine.Random.value < probability)
        {
           
            Vector2 position = transform.position;
            position.x= gameObject.transform.position.x +(spread*UnityEngine.Random.value - spread/2);
            position.y= gameObject.transform.position.y + (spread*UnityEngine.Random.value - spread/2);
            // GameObject go = Instantiate(pickUpDrop);
            // go.GetComponent<PickUpItem>().Set(item, itemCountInOneDrop);
            // go.transform.position=position;

            ItemSpawnManager.instance.SpawnItem(position,toSpawn,count);
        }
    }
}
