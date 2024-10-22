using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //==================ITEM DATA==================//
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDesc;
    public string itemID;
    public Sprite emptySprite;

    [SerializeField]
    private int maxNumberOfItems;

    //==================ITEM SLOT==================//
    [SerializeField]
    public TMP_Text quantityText;
    [SerializeField]
    public Image itemImage;

    //================ITEM DESC SLOT================//
    public Image itemDescImage;
    public TMP_Text itemDescNameText;
    public TMP_Text itemDescText;

    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("myCanvas").GetComponent<InventoryManager>();
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDesc)
    {
        if (isFull && this.itemName != itemName)
        {
            return quantity;
        }

        // Update Name
        this.itemName = itemName;
        // Update Image
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;
        // Update Description
        this.itemDesc = itemDesc;
        // Update Quantity
        this.quantity += quantity;
        if (this.quantity >= maxNumberOfItems)
        {
            quantityText.text = maxNumberOfItems.ToString();
            quantityText.enabled = true;
            isFull = true;

            // Return leftover items
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            return extraItems;
        }

        // Update quantity text
        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;
        return 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        inventoryManager.DeselectAllSlots();
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

    public void OnRightClick()
    {
        // Add functionality for right-click if needed
    }
    public void SetItem(string itemName, int quantity, Sprite itemSprite, string itemDesc)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        this.itemDesc = itemDesc;
        itemImage.sprite = itemSprite;
        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        isFull = true;
    }


}