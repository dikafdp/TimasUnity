using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public GameObject Inventory;
    private bool menuActivated;
    public ItemSlot[] itemSlot;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ResetInventory();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && menuActivated)
        {
            Time.timeScale = 1;
            Inventory.SetActive(false);
            menuActivated = false;
        }
        else if (Input.GetKeyDown(KeyCode.E) && !menuActivated)
        {
            Time.timeScale = 0f;
            Inventory.SetActive(true);
            menuActivated = true;
        }
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDesc)
    {
        if (itemName == "Koin")
        {
            PlayerController.Instance.AddGold(quantity); // Add gold directly to player
            return 0; // No leftover items
        }
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false && (itemSlot[i].itemName == itemName || itemSlot[i].quantity == 0))
            {
                int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDesc);
                if (leftOverItems > 0)
                {
                    leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDesc);
                }
                return leftOverItems;
            }
        }
        return quantity;
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }
    }

    public void ResetInventory()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].quantity = 0;
            itemSlot[i].isFull = false;
            itemSlot[i].itemName = "";
            itemSlot[i].itemSprite = null;
            itemSlot[i].itemDesc = "";
            itemSlot[i].itemImage.sprite = itemSlot[i].emptySprite;
            itemSlot[i].quantityText.text = "";
            itemSlot[i].quantityText.enabled = false;
        }
        PlayerPrefs.DeleteAll(); // Reset PlayerPrefs for all items
    }
    public void AddItemPrefab(GameObject itemPrefab)
    {
        Item item = itemPrefab.GetComponent<Item>();
        int remainingQuantity = AddItem(item.itemName, item.quantity, item.sprite, item.itemDesc);
        if (remainingQuantity <= 0)
        {
            Destroy(itemPrefab);
        }
        else
        {
            item.quantity = remainingQuantity;
        }
    }

}