using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable
{
    [SerializeField] dialog dialog;
    [SerializeField] private NpcType npcType = NpcType.Generic;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float walkTime = 1f;
    [SerializeField] private float waitTime = 2f;
    private Animator animator;
    private PlayerController playerController;
    private bool isWalking;
    private Vector2 movement;
    private Rigidbody2D rb;
    private ShopManager shopManager;
    private bool isPlayerInRange;

    public enum NpcType
    {
        Generic,
        Healer,
        Merchant
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        shopManager = FindObjectOfType<ShopManager>();

        if (animator != null)
        {
            StartCoroutine(WalkRoutine());
        }
    }

    private void Update()
    {
        if (isWalking)
        {
            rb.velocity = movement * moveSpeed;

            if (animator != null)
            {
                animator.SetFloat("moveX", movement.x);
                animator.SetFloat("moveY", movement.y);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", isWalking);
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Return)) // Check for Enter key press
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    public void Interact()
    {
        StartCoroutine(HandleInteraction());
    }

    private IEnumerator HandleInteraction()
    {
        StopCoroutine(WalkRoutine());
        isWalking = false;
        rb.velocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }

        yield return StartCoroutine(dialogManager.Instance.showDialog(dialog));

        if (npcType == NpcType.Healer && playerController != null)
        {
            playerController.InitializePlayer();
        }
        else if (npcType == NpcType.Merchant && shopManager != null)
        {
            shopManager.OpenShop();
        }

        StartCoroutine(WalkRoutine());
    }

    private IEnumerator WalkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            isWalking = true;
            movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            yield return new WaitForSeconds(walkTime);

            isWalking = false;
            rb.velocity = Vector2.zero;
        }
    }
}
