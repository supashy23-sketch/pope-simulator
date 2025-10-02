using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Sound Settings")]
    public AudioClip soundClip;   // ลากไฟล์เสียงมาใส่

    private AudioSource audioSource;

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

    private Vector2 baseAttackSize;
    public float attackWidthMultiplier = 1.5f; // ขยายแนวนอนเฉพาะ x

    [Header("Special Attack")]
    public GameObject lightProjectilePrefab; // prefab ของแสง/โปรเจคไทล์
    public Transform firePoint;               // จุดปล่อย projectile
    public float projectileSpeed = 10f;


    [Header("Light Charge")]
    public int maxLight = 10;       // พลังสูงสุด
    public int currentLight = 0;    // พลังปัจจุบัน
    public float chargeTime = 3f;   // เวลากดขวาค้างเพื่อเริ่มชาร์จ
    public Slider lightSlider;      // UI Slider แสดงพลัง
    public int lightDecayPerSecond = 1; // ลดพลังต่อวินาที

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private bool isAttacking;
    [HideInInspector] public bool isCharging = false; // ให้ PlayerLightController เข้ามาเช็คได้
    //private bool isCharging;
    private float moveX;
    private float chargeCounter = 0f;
    private float lightDecayCounter = 0f;
    private float lightGainCounter = 0f; // ตัวจับเวลาเพิ่มพลัง

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        baseAttackSize = attackSize;

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
        
        // ปรับ attackSize เฉพาะแนวนอน
        if (currentLight > 0)
        {
            attackSize.x = baseAttackSize.x * attackWidthMultiplier; // ขยายเฉพาะ x
            attackSize.y = baseAttackSize.y; // สูงเหมือนเดิม
        }
        else
        {
            attackSize = baseAttackSize;
        }

        // Attack → คลิกซ้าย
        if (Input.GetMouseButtonDown(0) && !isAttacking)
            Attack();

        // Charge Light → คลิกขวาค้าง
        if (Input.GetMouseButton(1))  // <<== คลิกขวา
        {
            if (!isCharging) isCharging = true;
            chargeCounter += Time.deltaTime;

            // เล่น Animation Charging
            if (animator != null)
                animator.SetBool("isCharging", true);

            // ถ้าชาร์จครบเวลาแล้ว → เริ่มเพิ่มพลังทีละ 1 ต่อวินาที
            if (chargeCounter >= chargeTime)
            {
                lightGainCounter += Time.deltaTime;
                if (lightGainCounter >= 1f)
                {
                    currentLight += 1;
                    if (currentLight > maxLight) currentLight = maxLight;

                    if (lightSlider != null)
                        lightSlider.value = currentLight;

                    lightGainCounter = 0f;
                }
            }
        }
        else
        {
            // ปล่อยคลิก → รีเซ็ตตัวจับเวลา
            isCharging = false;
            chargeCounter = 0f;
            lightGainCounter = 0f;
            if (animator != null)
                animator.SetBool("isCharging", false);
        }

        // ลดพลัง Light ทีละวินาที (เฉพาะตอนที่ไม่ได้ชาร์จ)
        if (currentLight > 0 && !isCharging)
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
        if (soundClip != null)
        {
            audioSource.PlayOneShot(soundClip);
        }

        Vector2 attackPos = (Vector2)transform.position + new Vector2(Mathf.Sign(transform.localScale.x) * attackOffset.x, attackOffset.y);
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPos, attackSize, 0f, enemyLayer);

        int damage = (currentLight > 0) ? 5 : 1; 

        foreach (Collider2D enemy in hitEnemies)
        {
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
            float knockbackForce = 5f;
            enemy.GetComponent<EnemyController>()?.TakeDamage(damage, knockbackDir, knockbackForce);
        }

        // ถ้า currentLight > 1 ปล่อย projectile
        if (currentLight > 1 && lightProjectilePrefab != null && firePoint != null)
        {
            GameObject proj = Instantiate(lightProjectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();
            if (rbProj != null)
                rbProj.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * projectileSpeed, 0f);
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
