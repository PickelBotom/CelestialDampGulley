using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCuttable : ToolHit
{
    [SerializeField] GameObject pickUpDrop;
    [SerializeField] int dropCount=3;
    [SerializeField] float spread=0.4f;
    public override void Hit()
    {
        while (dropCount>0)
        {
            dropCount--;
            Vector2 position = transform.position;
            position.x= gameObject.transform.position.x +(spread*UnityEngine.Random.value - spread/2);
            position.y= gameObject.transform.position.y + (spread*UnityEngine.Random.value - spread/2);
            GameObject go = Instantiate(pickUpDrop);
            go.transform.position=position;
        }
        Destroy(gameObject);
    }
}
