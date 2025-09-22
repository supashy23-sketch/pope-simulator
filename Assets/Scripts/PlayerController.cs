using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Attack")]
    public Vector2 attackOffset = new Vector2(0.5f, 0f);
    public Vector2 attackSize = new Vector2(1f, 0.5f);
    public LayerMask enemyLayer;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private bool isAttacking;
    private float moveX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");

        // ไม่ล็อค Movement แบบเต็ม
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        // พลิก Sprite
        if (moveX != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveX), 1, 1);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // Attack
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
            Attack();

        // Animator
        animator.SetFloat("speed", Mathf.Abs(moveX));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isAttacking", isAttacking);
    }

    private void FixedUpdate()
    {
        // ตรวจสอบพื้น
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // เรียกตอนกดโจมตี
    private void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("attack"); // Animator Trigger → Attack Animation
    }

    // ─── เรียกจาก Animation Event ───

    // ฟันดาบจริง
    public void DoAttack()
    {
        Vector2 attackPos = (Vector2)transform.position + new Vector2(Mathf.Sign(transform.localScale.x) * attackOffset.x, attackOffset.y);
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPos, attackSize, 0f, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            // ส่งทิศทาง Knockback ไปด้วย
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
            enemy.GetComponent<EnemyController>()?.TakeDamage(1, knockbackDir);
        }
    }

    // จบ Animation Attack → รีเซ็ตการโจมตี
    public void FinishAttack()
    {
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        // GroundCheck
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Attack Hitbox
        Gizmos.color = Color.yellow;
        Vector2 attackPos = (Vector2)transform.position + new Vector2(Mathf.Sign(transform.localScale.x) * attackOffset.x, attackOffset.y);
        Gizmos.DrawWireCube(attackPos, attackSize);
    }
}
