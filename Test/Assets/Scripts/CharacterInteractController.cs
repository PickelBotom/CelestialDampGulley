using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractController : MonoBehaviour
{
    PlayerController player;
    Rigidbody2D rb2d;
    [SerializeField] float offsetDistance=0.11f;
   [SerializeField] float sizeOFInteractArea=0.2f;
   Character character;
    private void Awake()
    {
       player = GetComponent<PlayerController>();
       rb2d=GetComponent<Rigidbody2D>();
       character = GetComponent<Character>();
    }

private void Update()
   {
    if(Input.GetMouseButtonDown(0))
    {
        Debug.Log("Left click pressed");
        interact();
    }
   }
    private void interact()
  {
    Vector2 pos = rb2d.position + player.LastMotionVector * offsetDistance;
    Collider2D[] colliders = Physics2D.OverlapCircleAll(pos,sizeOFInteractArea);

    foreach(Collider2D c in colliders)
    {
        Interactable hit = c.GetComponent<Interactable>();
        if(hit!= null)
        {
            hit.Interact(character);
            break;
        }
    }
  }
}
