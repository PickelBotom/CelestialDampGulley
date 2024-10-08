using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ResourceNode : ToolHit
{
    [SerializeField] GameObject pickUpDrop;
    [SerializeField] int dropCount=3;
    [SerializeField] float spread=0.4f;
    [SerializeField] Item item;
    [SerializeField] int itemCountInOneDrop = 5;
    [SerializeField] ResourceNodeType nodeType;
    public override void Hit()
    {
        while (dropCount>0)
        {
            dropCount--;
            Vector2 position = transform.position;
            position.x= gameObject.transform.position.x +(spread*UnityEngine.Random.value - spread/2);
            position.y= gameObject.transform.position.y + (spread*UnityEngine.Random.value - spread/2);
            GameObject go = Instantiate(pickUpDrop);
            go.GetComponent<PickUpItem>().Set(item, itemCountInOneDrop);
            go.transform.position=position;

            ItemSpawnManager.instance.SpawnItem(position,item,itemCountInOneDrop);
        }
        Destroy(gameObject);
    }

    public override bool CanBeHit(List<ResourceNodeType> canBeHit)
    {
        return canBeHit.Contains(nodeType);
    }
}
