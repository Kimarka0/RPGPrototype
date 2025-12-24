using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;
    private ItemDictionary itemDictionary;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
        itemDictionary = FindAnyObjectByType<ItemDictionary>();
    }

    // Update is called once per frame
   private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if(item != null)
            {
                // bool itemAdded = inventoryController.AddItem(collision.gameObject);
                GameObject itemPrefab = itemDictionary.GetItemPrefab(item.ID);

                if (itemPrefab != null && inventoryController.AddItem(itemPrefab))
                {
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}
