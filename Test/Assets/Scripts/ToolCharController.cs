//using System.Threading.Tasks.Dataflow;
//using System.Numerics;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ToolCharController : MonoBehaviour
{
   PlayerController player;
   Rigidbody2D rb2d;
   ToolBarController toolBarController;
   Animator animator;
   
   [SerializeField] float offsetDistance=0.11f;
   [SerializeField] float sizeOFInteractArea=0.2f;
   [SerializeField] MarkerManager markerManager;//ep8
   [SerializeField] TileMapReadController tileMapReadController;//ep8
   [SerializeField] float maxDistance = 0.5f;

   Vector3Int selectedTilePosition;//ep9
   bool selectable;//ep9
   
    private void Awake()
   {
    player = GetComponent<PlayerController>();
    rb2d=GetComponent<Rigidbody2D>();
    toolBarController = GetComponent<ToolBarController>();
    animator = GetComponent<Animator>();
   }

   private void Update()
   {

    SelectTile();
    CanSelectCheck();
    Marker();//ep8
    if(Input.GetMouseButtonDown(1))
    {
        Debug.Log("Right click pressed");
        if(UseToolWorld())
        {return;}
        UseToolGrid();
    }
   }

   private void SelectTile()
   {
    selectedTilePosition = tileMapReadController.GetGridPosition(Input.mousePosition,true);
   }

   void CanSelectCheck()
   {
    Vector2 charPos=transform.position;
    Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    selectable = Vector2.Distance(charPos,cameraPos)<maxDistance;
    markerManager.Show(selectable);
   }
  private void Marker() // ep 8
  {
  // Vector3Int gridPosition = tileMapReadController.GetGridPosition(Input.mousePosition,true); - ep 9 edit
  markerManager.markedCellPosition= selectedTilePosition;
  }

  private bool UseToolWorld()
  {
    Vector2 pos = rb2d.position + player.LastMotionVector * offsetDistance;
    Item item = toolBarController.GetItem;
    if(item == null){return false;}
    if(item.OnAction == null){return false;}

    animator.SetTrigger("Act");
    bool complete = item.OnAction.OnApply(pos);

    if(complete == true){

      if(item.onItemUsed != null){
        item.onItemUsed.OnItemUsed(item, GameManager.instance.inventoryContainer);
      }
    }

    return complete;
  }

  private void UseToolGrid()
  {
    if(selectable == true)
    {
      Item item = toolBarController.GetItem;
      if(item == null){return;}
      if(item.onTileMapAction == null){return;}

      animator.SetTrigger("Act");
      bool complete = item.onTileMapAction.OnApplyToTileMap(selectedTilePosition, tileMapReadController);

      if(complete == true){

        if(item.onItemUsed != null){
          item.onItemUsed.OnItemUsed(item, GameManager.instance.inventoryContainer);
        }
      }
    }
  }
}
