using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemNoPlayerPrefs : MonoBehaviour
{
    [SerializeField]
    private string itemName;

    [SerializeField]
    private int quantity;

    [SerializeField]
    private Sprite sprite;

    [TextArea]
    [SerializeField]
    private string itemDesc;

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
    [SerializeField]
    private int goldBoost;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GameObject.Find("myCanvas").GetComponent<InventoryManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            int leftOverItems = inventoryManager.AddItem(itemName, quantity, sprite, itemDesc);
            if (leftOverItems <= 0)
            {
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
            player.IncreaseMoveSpeed(speedBoost);  // Apply speed boost
            player.AddGold(goldBoost);
        }
    }
}
