//using System.Threading.Tasks.Dataflow;
//using System.Numerics;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class ToolCharController : MonoBehaviour
{
   PlayerController player;
   Rigidbody2D rb2d;
   
   [SerializeField] float offsetDistance=0.11f;
   [SerializeField] float sizeOFInteractArea=0.2f;
   [SerializeField] MarkerManager markerManager;//ep8
   [SerializeField] TileMapReadController tileMapReadController;//ep8
   [SerializeField] float maxDistance = 0.5f;
   [SerializeField] CropManager cropsManager;
   [SerializeField] TileData PlowableTiles;

   Vector3Int selectedTilePosition;//ep9
   bool selectable;//ep9
   
    private void Awake()
   {
    player = GetComponent<PlayerController>();
    rb2d=GetComponent<Rigidbody2D>();
   }

   private void Update()
   {

    SelectTile();
    CanSelectCheck();
    Marker();//ep8
    if(Input.GetMouseButtonDown(1))
    {
        Debug.Log("Right click presed");
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
    Collider2D[] colliders = Physics2D.OverlapCircleAll(pos,sizeOFInteractArea);

    foreach(Collider2D c in colliders)
    {
        ToolHit hit = c.GetComponent<ToolHit>();
        if(hit!= null)
        {
            hit.Hit();
            return true;
        }
    }
    return false;
  }

  private void UseToolGrid()
  {
    if(selectable == true)
    {
      TileBase tileBase = tileMapReadController.GetTileBase(selectedTilePosition);
      TileData tileData = tileMapReadController.GetTileData(tileBase);
      
      if (tileData != PlowableTiles){ return;}
      if(cropsManager.CheckTile(selectedTilePosition))
      {
        cropsManager.Seed(selectedTilePosition);
      }else
      {
      cropsManager.Plow(selectedTilePosition);
      }
    }
  }
}
