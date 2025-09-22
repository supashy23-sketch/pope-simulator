using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int health = 3;
    public float moveSpeed = 2f;
    public float followRange = 5f;    // ระยะที่เริ่มตาม Player
    public float knockbackForce = 5f; // แรงกระเด็นเมื่อโดนโจมตี

    private float knockbackTimer = 0f;
    public float knockbackDuration = 0.2f; // ระยะเวลาให้กระเด็น

    private Vector2 knockbackVelocity;

    private Transform player;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        // ลดเวลา knockback
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            rb.velocity = knockbackVelocity;
            return; // ขณะกระเด็นไม่ทำ Movement ปกติ
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

    public void TakeDamage(int dmg, Vector2 knockbackDir)
    {
        health -= dmg;

        // กำหนด Knockback
        knockbackVelocity = new Vector2(knockbackDir.x * knockbackForce, knockbackDir.y * knockbackForce);
        knockbackTimer = knockbackDuration;

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
