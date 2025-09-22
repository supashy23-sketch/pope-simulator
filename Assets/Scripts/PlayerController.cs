using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Light Charge")]
    public int maxLight = 10;       // พลังสูงสุด
    public int currentLight = 0;    // พลังปัจจุบัน
    public float chargeTime = 3f;   // เวลากดขวาค้างเพื่อชาร์จเต็ม
    public Slider lightSlider;      // UI Slider แสดงพลัง
    public int lightDecayPerSecond = 1; // ลดพลังต่อวินาที

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private bool isAttacking;
    private bool isCharging;
    private float moveX;
    private float chargeCounter = 0f;
    private float lightDecayCounter = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // ตั้งค่า UI Slider
        if (lightSlider != null)
        {
            lightSlider.maxValue = maxLight;
            lightSlider.value = currentLight;
        }
    }

    private void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");

        // Movement
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        // พลิก Sprite
        if (moveX != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveX), 1, 1);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // Attack → คลิกซ้าย
        if (Input.GetMouseButtonDown(0) && !isAttacking)
            Attack();

        // Charge Light → คลิกขวาค้าง
        if (Input.GetMouseButton(1))
        {
            if (!isCharging) isCharging = true;
            chargeCounter += Time.deltaTime;

            // เล่น Animation Charging
            if (animator != null)
                animator.SetBool("isCharging", true);

            // ชาร์จครบ
            if (chargeCounter >= chargeTime)
            {
                currentLight = maxLight;
                if (lightSlider != null)
                    lightSlider.value = currentLight;

                chargeCounter = 0f;
                isCharging = false;
                if (animator != null)
                    animator.SetBool("isCharging", false);
            }
        }
        else
        {
            // ปล่อยคลิก → รีเซ็ตชาร์จ
            isCharging = false;
            chargeCounter = 0f;
            if (animator != null)
                animator.SetBool("isCharging", false);
        }

        // ลดพลัง Light ทีละวินาที
        if (currentLight > 0)
        {
            lightDecayCounter += Time.deltaTime;
            if (lightDecayCounter >= 1f)
            {
                currentLight -= lightDecayPerSecond;
                if (currentLight < 0) currentLight = 0;
                if (lightSlider != null)
                    lightSlider.value = currentLight;
                lightDecayCounter = 0f;
            }
        }

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

    private void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("attack");
    }

    // เรียกจาก Animation Event → ฟันจริง
    public void DoAttack()
    {
        Vector2 attackPos = (Vector2)transform.position + new Vector2(Mathf.Sign(transform.localScale.x) * attackOffset.x, attackOffset.y);
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPos, attackSize, 0f, enemyLayer);

        int damage = (currentLight > 0) ? 5 : 1; // ถ้ามีพลัง → ดาเมจ 5, ปกติ → 1

        foreach (Collider2D enemy in hitEnemies)
        {
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;

            // ส่งแรง Knockback (ใช้ค่าของ EnemyController หรือค่าที่กำหนด)
            float knockbackForce = 5f; // หรือปรับตามต้องการ
            enemy.GetComponent<EnemyController>()?.TakeDamage(damage, knockbackDir, knockbackForce);
        }
    }

    // เรียกจาก Animation Event → จบโจมตี
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
