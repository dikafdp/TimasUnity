using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopSlot : MonoBehaviour, IPointerClickHandler
{
    //==================ITEM DATA==================//
    public string itemName;
    public int price;
    public Sprite itemSprite;
    public string itemDesc;
    public GameObject itemPrefab; // Prefab for the item
    public Sprite emptySprite;


    //==================ITEM SLOT==================//
    [SerializeField]
    public TMP_Text priceText;
    [SerializeField]
    public Image itemImage;

    //================ITEM DESC SLOT================//
    public Image itemDescImage;
    public TMP_Text itemDescNameText;
    public TMP_Text itemDescText;

    public GameObject selectedShader;
    public bool thisItemSelected;

    public TextMeshProUGUI buyText;
    public TextMeshProUGUI purchasedText;

    private ShopManager shopManager;
    private InventoryManager inventoryManager;


    private void Start()
    {
        shopManager = GameObject.Find("ShopCanvas").GetComponent<ShopManager>();
        inventoryManager = GameObject.Find("myCanvas").GetComponent<InventoryManager>();
        if (shopManager == null)
        {
            Debug.LogError("ShopManager not found");
        }
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found");
        }

    }

    public void SetItem(string itemName, int price, Sprite itemSprite, string itemDesc, GameObject itemPrefab)
    {
        // Update item properties
        this.itemName = itemName;
        this.price = price;
        this.itemSprite = itemSprite;
        this.itemDesc = itemDesc;
        this.itemPrefab = itemPrefab;

        // Update UI
        itemImage.sprite = itemSprite;
        priceText.text = price.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    public void OnLeftClick()
    {
        shopManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        itemDescNameText.text = itemName;
        itemDescText.text = itemDesc;
        itemDescImage.sprite = itemSprite;
        if (itemDescImage.sprite == null)
        {
            itemDescImage.sprite = emptySprite;
        }
    }

    public void BuyItem()
    {
        purchasedText.gameObject.SetActive(false);

        Item item = itemPrefab.GetComponent<Item>();
        if (PlayerPrefs.GetInt(item.itemID, 0) == 1)
        {
            Debug.Log($"Item {itemName} already purchased.");
            purchasedText.gameObject.SetActive(true);
            return; // Prevent re-purchasing
        }

        if (PlayerController.Instance.gold >= item.price)
        {
            PlayerController.Instance.gold -= item.price;
            UIController.Instance.UpdateGoldUI(PlayerController.Instance.gold);
            inventoryManager.AddItemPrefab(itemPrefab);
            PlayerPrefs.SetInt(item.itemID, 1);
            PlayerPrefs.Save();
            Debug.Log($"Item {itemName} purchased and added to inventory.");
            purchasedText.gameObject.SetActive(true);
            buyText.gameObject.SetActive(false);

            ApplyItemStats(item);
        }
        else
        {
            Debug.Log("Not enough gold to buy the item.");
        }
    }
    private void ApplyItemStats(Item item)
    {
        PlayerController.Instance.IncreaseMaxHealth(item.healthIncrease);
        PlayerController.Instance.IncreaseAttackDamage(item.attackIncrease);
        PlayerController.Instance.IncreaseMoveSpeed(item.speedIncrease);
    }
}