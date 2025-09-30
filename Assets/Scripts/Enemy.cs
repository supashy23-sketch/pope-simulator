using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int health = 3;
    public int damage = 1;          // ปรับ damage แต่ละตัวใน Inspector
    public float moveSpeed = 2f;
    public float followRange = 5f;
    public float knockbackForce = 5f; // ใช้ส่งไป Player

    private float knockbackTimer = 0f;
    public float knockbackDuration = 0.2f;

    private Vector2 knockbackVelocity;

    private Transform player;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            rb.velocity = knockbackVelocity;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= followRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            if (direction.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public void TakeDamage(int dmg, Vector2 knockbackDir, float force)
    {
        health -= dmg;
        knockbackVelocity = new Vector2(knockbackDir.x * force, knockbackDir.y * force);
        knockbackTimer = knockbackDuration;

        if (health <= 0)
            Die();
    }

    public void TakeDamage(int dmg)
    {
        TakeDamage(dmg, Vector2.zero, 0f);
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    // ⚡ โจมตี Player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Vector2 knockDir = (collision.transform.position - transform.position).normalized;
                playerHealth.TakeDamage(damage, knockDir, knockbackForce);
            }
        }
    }
}
