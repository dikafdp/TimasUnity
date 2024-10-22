using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ShopSlot[] shopSlots; // Array of shop slots
    [SerializeField] private GameObject[] itemPrefabs; // Array of item prefabs

    public static ShopManager instance;

    public GameObject ShopUI;
    private bool menuActivated;

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
        ResetShop();
        InitializeShop();
    }

    public void InitializeShop()
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (i < itemPrefabs.Length)
            {
                Item item = itemPrefabs[i].GetComponent<Item>();
                shopSlots[i].SetItem(item.itemName, item.price, item.sprite, item.itemDesc, itemPrefabs[i]);
            }
        }
    }

    public void close()
    {
        if (menuActivated)
        {
            CloseShop();
        }
    }

    public void OpenShop()
    {
        Time.timeScale = 0f;
        ShopUI.SetActive(true);
        menuActivated = true;
    }

    public void CloseShop()
    {
        Time.timeScale = 1f;
        ShopUI.SetActive(false);
        menuActivated = false;
    }

    public void DeselectAllSlots()
    {
        foreach (var slot in shopSlots)
        {
            slot.selectedShader.SetActive(false);
            slot.thisItemSelected = false;
        }
    }

    public void ResetShop()
    {
        foreach (var slot in shopSlots)
        {
            slot.itemName = "";
            slot.price = 0;
            slot.itemSprite = null;
            slot.itemDesc = "";
            slot.itemImage.sprite = slot.emptySprite;
            slot.priceText.text = "";
        }
    }

    public void BuySelectedItem()
    {
        foreach (var slot in shopSlots)
        {
            if (slot.thisItemSelected)
            {
                Debug.Log($"Buying item: {slot.itemName}");
                slot.BuyItem();
                break;
            }
        }
    }

}