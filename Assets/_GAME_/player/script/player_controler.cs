using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int maxHealth = 100;
    public LayerMask solidObjectLayer;
    public LayerMask waterLayer;
    public LayerMask interactableLayer;
    public LayerMask enemyLayer;
    public int attackDamage = 5;
    public float attackDuration = 1f;
    public UIController uiController;
    public healthBar healthBar;
    public AudioSource walkAudioSource;
    public AudioSource attackAudioSource;
    public AudioClip walkClip;
    public AudioClip attackClip;
    public float walkVolume = 0.2f;
    public float attackVolume = 0.2f;
    public int gold = 0;

    private bool isMoving;
    private bool isAttacking;
    private bool isAlive = true;
    private Vector2 movement;
    private Animator animator;
    private Rigidbody2D rb;
    private int currentHealth;

    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        attackAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.clip = walkClip;
        attackAudioSource.clip = attackClip;

        walkAudioSource.volume = walkVolume;
        attackAudioSource.volume = attackVolume;
    }

    private void Start()
    {
        InitializePlayer();
    }

    public void OnSceneChange()
    {
        uiController = FindObjectOfType<UIController>();
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    public void ResetPosition()
    {
        if (gameController.Instance != null)
        {
            transform.position = gameController.Instance.DefaultPlayerPosition;
            gameController.Instance.PlayerPosition = gameController.Instance.DefaultPlayerPosition;
        }
    }

    public void InitializePlayer()
    {
        if (GameManager.Instance != null)
        {
            maxHealth = GameManager.Instance.maxHealth;
            GameManager.Instance.currentHealth = maxHealth;
            healthBar.Instance.SetMaxHealth(maxHealth);
            healthBar.Instance.SetHealth(GameManager.Instance.currentHealth);
        }

        currentHealth = maxHealth;
        isAlive = true;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }

        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
        }
    }

    public void handleUpdate()
    {
        if (!isAlive) return; // Prevent movement if the player is not alive

        if (!isAttacking)
        {
            HandleMovementInput();
        }

        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Interact();
        }
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private void FixedUpdate()
    {
        if (!isAlive) return; // Prevent movement if the player is not alive

        if (!isAttacking && isMoving)
        {
            Move();
        }
    }

    private void HandleMovementInput()
    {
        if (!isAlive) return; // Prevent movement if the player is not alive

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        bool wasMoving = isMoving;
        isMoving = movement != Vector2.zero;

        if (isMoving)
        {
            movement = movement.normalized;
            animator.SetFloat("moveX", movement.x);
            animator.SetFloat("moveY", movement.y);

            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }
        }
        else
        {
            walkAudioSource.Stop();
        }

        animator.SetBool("isMoving", isMoving);
    }

    private void Move()
    {
        Vector2 targetPos = rb.position + movement * moveSpeed * Time.fixedDeltaTime;

        if (IsWalkable(targetPos))
        {
            rb.MovePosition(targetPos);
        }
    }

    private void Interact()
    {
        Vector3 facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        Vector3 interactPos = transform.position + facingDir;

        Collider2D collider = Physics2D.OverlapCircle(interactPos, 0.4f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }

    }

    private IEnumerator Attack()
    {
        if (!isAlive) yield break; // Prevent attacking if the player is not alive

        isAttacking = true;
        animator.SetBool("isAttacking", true);

        if (attackAudioSource != null && attackAudioSource.clip != null)
        {
            attackAudioSource.Play();
        }

        Vector3 facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        Vector3 attackPos = transform.position + facingDir;

        Collider2D enemyCollider = Physics2D.OverlapCircle(attackPos, 0.5f, enemyLayer);
        if (enemyCollider != null)
        {
            enemyCollider.GetComponent<enemyController>()?.EnemyTakeDamage(attackDamage);
        }
        if (enemyCollider != null)
        {
            enemyCollider.GetComponent<monsterController>()?.EnemyTakeDamage(attackDamage);
        }


        yield return new WaitForSeconds(attackDuration);

        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectLayer | waterLayer) == null;
    }

    public void PlayerTakeDamage(int damage)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentHealth -= damage;
            if (healthBar.Instance != null)
            {
                healthBar.Instance.SetHealth(GameManager.Instance.currentHealth);
            }

            if (GameManager.Instance.currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        Debug.Log("You died");
        isAlive = false; // Set alive state to false
        if (UIController.Instance != null)
        {
            UIController.Instance.ShowDefeatPanel();
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
            GameManager.Instance.currentHealth = GameManager.Instance.maxHealth;
            if (healthBar.Instance != null)
            {
                healthBar.Instance.SetHealth(GameManager.Instance.currentHealth);
            }
        }
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.ResetInventory(); // Reset inventory on player death
        }
        StartCoroutine(ReloadScene());
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(1f);
        if (UIController.Instance != null)
        {
            UIController.Instance.ShowDefeatPanel();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance != null && healthBar.Instance != null)
        {
            healthBar.Instance.SetHealth(GameManager.Instance.currentHealth);
        }
        if (scene.buildIndex == 0)
        {
            InitializePlayer(); // Initialize player on scene load
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        //currentHealth = maxHealth; // Optional: Reset current health to new max health
        /*if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }*/
    }

    public void IncreaseAttackDamage(int amount)
    {
        attackDamage += amount;
    }

    public void DecreaseAttackDuration(float amount)
    {
        attackDuration -= amount;
        if (attackDuration < 0.1f) // Optional: Set a minimum limit to attack duration
        {
            attackDuration = 0.1f;
        }
    }

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }
    public void AddGold(int amount)
    {
        gold += amount;
        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateGoldUI(gold);
        }
    }

    public void SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            if (UIController.Instance != null)
            {
                UIController.Instance.UpdateGoldUI(gold);
            }
        }
        else
        {
            Debug.Log("Not enough gold to spend");
        }
    }
}

