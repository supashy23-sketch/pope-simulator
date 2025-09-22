using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;       // จุดเช็คพื้น (สร้าง Empty ไว้ใต้เท้าตัวละคร)
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private float moveX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // รับค่าปุ่มซ้าย-ขวา
        moveX = Input.GetAxisRaw("Horizontal");

        // เดินซ้าย-ขวา
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        // พลิกตัวตามทิศทาง
        if (moveX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveX), 1, 1);
        }

        // กระโดด
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // อัปเดต Animator
        animator.SetFloat("speed", Mathf.Abs(moveX));
        animator.SetBool("isGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        // เช็คว่าตัวละครยืนบนพื้นไหม
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // สำหรับ Debug
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
