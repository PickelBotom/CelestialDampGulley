using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class ItemDragAnDropContainer : MonoBehaviour
{

    [SerializeField] ItemSlot itemSlot;
    [SerializeField] GameObject itemIcon;
    RectTransform iconTransform;
    Image itemIconImage;

    internal void OnClick(ItemSlot itemSlot)
    {
        if(this.itemSlot.item == null){
            this.itemSlot.Copy(itemSlot);
            itemSlot.Clear();
        }
        else{
            Item item = itemSlot.item;
            int count = itemSlot.count;

            itemSlot.Copy(this.itemSlot);
            this.itemSlot.Set(item,count);
            
        }
        UpdateIcon();

    }

    private void UpdateIcon()
    {
        if(itemSlot.item == null){
            itemIcon.SetActive(false);
        }
        else{
            itemIcon.SetActive(true);
            itemIconImage.sprite = itemSlot.item.icon;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (itemIcon == null)
        {
            Debug.LogError("ItemIcon is not assigned in the Inspector.");
            return;
        }

        itemSlot = new ItemSlot();
        iconTransform = itemIcon.GetComponent<RectTransform>();
        itemIconImage = itemIcon.GetComponent<Image>();
        if (itemIconImage == null)
        {
            Debug.LogError("ItemIconImage component not found on itemIcon.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(itemIcon.activeInHierarchy == true){
            iconTransform.position = Input.mousePosition;
            if(Input.GetMouseButtonDown(0)){
               if(EventSystem.current.IsPointerOverGameObject()==false){
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    worldPosition.z = 0;
                    ItemSpawnManager.instance.SpawnItem(
                        worldPosition, 
                        itemSlot.item, 
                        itemSlot.count);

                        itemSlot.Clear();
                        itemIcon.SetActive(false);
                }                  
            }
        }
    }
}
