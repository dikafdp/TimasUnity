using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    public string itemName;

    [SerializeField]
    public int quantity;

    [SerializeField]
    public Sprite sprite;

    [SerializeField]
    public string itemID;

    [SerializeField]
    private int healthBoost;
    [SerializeField]
    private int damageBoost;
    [SerializeField]
    private float durationBoost;
    [SerializeField]
    private float speedBoost;

    public int healthIncrease;
    public int attackIncrease;
    public float speedIncrease;

    [SerializeField]
    public int price; // Price of the item

    [TextArea]
    [SerializeField]
    public string itemDesc;

    private InventoryManager inventoryManager;

    private static HashSet<string> purchasedItems = new HashSet<string>();

    void Start()
    {
        inventoryManager = GameObject.Find("myCanvas").GetComponent<InventoryManager>();
        if (PlayerPrefs.GetInt(itemID, 0) == 1)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (purchasedItems.Contains(itemID))
            {
                return; // Item has already been purchased
            }
            int leftOverItems = inventoryManager.AddItem(itemName, quantity, sprite, itemDesc);
            if (leftOverItems <= 0)
            {
                PlayerPrefs.SetInt(itemID, 1);
                PlayerPrefs.Save();
                ApplyItemStats(collision.gameObject.GetComponent<PlayerController>());
                Destroy(gameObject);
            }
            else
            {
                quantity = leftOverItems;
            }
        }
    }
    private void ApplyItemStats(PlayerController player)
    {
        if (player != null)
        {
            player.IncreaseMaxHealth(healthBoost);
            player.IncreaseAttackDamage(damageBoost);
            player.DecreaseAttackDuration(durationBoost);
            player.IncreaseMoveSpeed(speedBoost);
        }
    }
}