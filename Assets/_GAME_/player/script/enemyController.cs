using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyController : MonoBehaviour
{
    public int maxHealth = 200;
    private int currentHealth;
    public float moveSpeed = 4f;
    public float chaseRange = 12f;
    public float attackRange = 2f;
    public int attackDamage = 10;
    public float attackRate = 1f;
    public float cooldownTime = 2f;
    public GameObject deathEffect;
    public UIController uiController;
    public EnemyHealthBar healthBar;
    public AudioSource roarAudioSource;
    public AudioSource stepAudioSource;
    public AudioSource punchAudioSource;
    public AudioClip roarClip;
    public AudioClip stepClip;
    public AudioClip punchClip;
    public float sfxVolume = 0.2f;
    public GameObject itemPrefab;
    public string dropItemID;

    private Transform player;
    private Animator animator;
    private bool isAttacking;
    private bool isOnCooldown;
    private Rigidbody2D rb;
    private Coroutine roarCoroutine;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        // Initialize AudioSource components
        roarAudioSource = gameObject.AddComponent<AudioSource>();
        stepAudioSource = gameObject.AddComponent<AudioSource>();
        punchAudioSource = gameObject.AddComponent<AudioSource>();

        // Assign clips to AudioSource components
        roarAudioSource.clip = roarClip;
        stepAudioSource.clip = stepClip;
        punchAudioSource.clip = punchClip;

        // Set volume levels
        roarAudioSource.volume = sfxVolume;
        stepAudioSource.volume = sfxVolume;
        punchAudioSource.volume = sfxVolume;

        roarCoroutine = StartCoroutine(RoarRoutine());
    }


    private void Update()
    {
        if (player != null)
        {
            Vector3 playerPosition = player.position;

            // Calculate the distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
            if (distanceToPlayer <= chaseRange && !isOnCooldown)
            {
                if (distanceToPlayer > attackRange)
                {
                    MoveTowardsPlayer(playerPosition);
                }
                else if (!isAttacking)
                {
                    StartCoroutine(PrepareAndAttack());
                }
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    void MoveTowardsPlayer(Vector3 playerPosition)
    {
        if (isAttacking) return;

        Vector3 direction = (playerPosition - transform.position).normalized;
        Vector2 newPosition = Vector2.MoveTowards(rb.position, playerPosition, moveSpeed * Time.deltaTime);

        rb.MovePosition(newPosition);

        animator.SetBool("isMoving", true); // Enable the walk animation

        // Set animator parameters based on movement direction
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);

        // Play step sound
        if (!stepAudioSource.isPlaying)
        {
            stepAudioSource.Play();
        }
    }


    public void EnemyTakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        if (roarCoroutine != null)
        {
            StopCoroutine(roarCoroutine);
        }

        DropItem();

        Destroy(gameObject);
        Debug.Log("Enemy died");
        if (UIController.Instance != null)
        {
            UIController.Instance.ShowVictoryPanel();
        }
    }
    void DropItem()
    {
        if (itemPrefab != null)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            Item item = droppedItem.GetComponent<Item>();
            if (item != null)
            {
                item.itemID = dropItemID;
            }
        }
    }

    IEnumerator PrepareAndAttack()
    {
        isAttacking = true;
        animator.SetBool("isMoving", false); // Ensure enemy is idle before attacking

        yield return new WaitForSeconds(0.5f); // Short delay before attack

        animator.SetBool("isAttacking", true);

        // Play punch sound
        punchAudioSource.Play();

        // Wait for half of the attack duration (synchronize with animation)
        yield return new WaitForSeconds(0.5f / attackRate);

        // Check if the player is still within attack range before dealing damage
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                // Assuming the player has a method TakeDamage(int damage)
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.PlayerTakeDamage(attackDamage);
                }
            }
        }

        // Wait for the rest of the attack duration
        yield return new WaitForSeconds(0.5f / attackRate);

        animator.SetBool("isAttacking", false);
        isAttacking = false;
        StartCoroutine(Cooldown());
    }


    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime); // Cooldown time between attacks
        isOnCooldown = false;
    }
    IEnumerator RoarRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            roarAudioSource.Play();
        }
    }

}
