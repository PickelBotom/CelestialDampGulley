using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class ItemSlot{
    public Item item;
    public int count;
    
}
[CreateAssetMenu(menuName ="Data/Item Container")]
public class ItemContainer : ScriptableObject
{

    public List<ItemSlot> slots;

    public void Add(Item item, int count = 1){
        if(item.stackable == true){
            ItemSlot itemSlot = slots.Find(x => x.item==item);
            if(itemSlot != null){
                itemSlot.count += count;
            }
            else{
                itemSlot = slots.Find(x => x.item == null);
                if(itemSlot != null){
                    itemSlot.item = item;
                    itemSlot.count = count;
                }
            }
        }
        else{
            //adding non stackable item to container
            ItemSlot itemSlot = slots.Find(x => x.item == null);
            if(itemSlot != null){
                itemSlot.item = item;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
