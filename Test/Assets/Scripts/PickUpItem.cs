using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
  // public GameObject player ---works;
    GameObject player;
   [SerializeField] float speed= 1f;
   [SerializeField] float pickUpDistance= 1.5f;
   [SerializeField] float ttl= 10f;// time to live
   public Item item;
   public int count = 1;

    bool inrange=false;
    float distance;

    private void Start()
    {
        player = GameManager.instance.player;
    }

    public void Set(Item item, int count){
        this.item = item;
        this.count = count;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = item.icon;
    }
   
   private void Update()
   {

      ttl -=Time.deltaTime;
      if(ttl <0 && inrange==false){ Destroy(gameObject);}

      distance = Vector3.Distance(transform.position,player.transform.position);
        if (distance>pickUpDistance)
        {
            return;
        }

      inrange=true;    
      Vector3 direction = player.transform.position - transform.position;

      transform.position = Vector3.MoveTowards(this.transform.position,player.transform.position,speed* Time.deltaTime);
     if(distance<0.1f)
     {
      if(GameManager.instance.inventoryContainer != null){
        GameManager.instance.inventoryContainer.Add(item, count);
      }
      else{
        Debug.LogWarning("No inventory container attached to game manager");
      }
      Destroy(gameObject);
     }
   }

}
