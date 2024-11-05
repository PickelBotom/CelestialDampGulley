using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    GameObject player;
    [SerializeField] float speed = 1f;
    [SerializeField] float pickUpDistance = 1.5f;
    [SerializeField] float ttl = 15f; // time to live
    public Item item;
    public int count = 1;

    private bool inRange = false;
    private float distance;

    private EnvironmentManager environmentManager; // Reference to EnvironmentManager

    private void Start()
    {
        player = GameManager.instance.player;
        environmentManager = FindObjectOfType<EnvironmentManager>(); // Find the EnvironmentManager instance
    }

    public void Set(Item item, int count)
    {
        this.item = item;
        this.count = count;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = item.icon;
    }

    private void Update()
    {
        ttl -= Time.deltaTime;
        if (ttl < 0)
        {
            Destroy(gameObject); // Destroy if TTL is reached, regardless of inRange
            return;
        }

        distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > pickUpDistance)
        {
            inRange = false; // Reset inRange if the player is not close
            return;
        }

        inRange = true; // Set inRange to true since the player is close
        Vector3 direction = player.transform.position - transform.position;

        // Move the item towards the player
        transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
        
        if (distance < 0.1f)
        {
            // Attempt to add item to inventory
            if (GameManager.instance.inventoryContainer != null)
            {
                GameManager.instance.inventoryContainer.Add(item, count);

                // Call PickUpTrash method from EnvironmentManager to update environmental health
                environmentManager?.PickUpTrash(); // Ensure this method is called

                Debug.Log("Picked up item: " + item.name); // Log the item picked up for debugging
            }
            else
            {
                Debug.LogWarning("No inventory container attached to game manager");
            }
            Destroy(gameObject);
        }
    }
}
