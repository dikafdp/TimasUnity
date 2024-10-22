using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterController : MonoBehaviour
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
    public GameObject[] itemPrefabs;
    public float[] dropRates;

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
        Vector2 targetPosition = (Vector2)transform.position + (Vector2)direction * moveSpeed * Time.deltaTime;

        if (IsPathClear(targetPosition))
        {
            rb.MovePosition(targetPosition);

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
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    bool IsPathClear(Vector2 targetPosition)
    {
        // Use a small box cast to detect collisions with colliders
        Vector2 currentPosition = rb.position;
        Vector2 direction = targetPosition - currentPosition;
        float distance = direction.magnitude;

        RaycastHit2D hit = Physics2D.BoxCast(currentPosition, rb.GetComponent<BoxCollider2D>().size, 0, direction, distance, LayerMask.GetMask("solidObject"));

        return hit.collider == null;
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
    }

    void DropItem()
    {
        if (itemPrefabs != null && itemPrefabs.Length > 0 && dropRates != null && dropRates.Length == itemPrefabs.Length)
        {
            float total = 0;
            for (int i = 0; i < dropRates.Length; i++)
            {
                total += dropRates[i];
            }

            float randomPoint = Random.value * total;

            for (int i = 0; i < itemPrefabs.Length; i++)
            {
                if (randomPoint < dropRates[i])
                {
                    Instantiate(itemPrefabs[i], transform.position, Quaternion.identity);
                    break;
                }
                else
                {
                    randomPoint -= dropRates[i];
                }
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
