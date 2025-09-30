using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("UI Elements")]
    public Slider healthBar;
    public string sceneName;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;      // ปรับแรงรวมจาก Inspector
    public float knockbackDuration = 0.2f;

    [Header("Door Interaction")]
    public GameObject targetObject; // อันนี้จะถูกเปิด/ปิดเมื่อเข้าออกประตู
    private Rigidbody2D rb;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    // ฟังก์ชันโดนโจมตี + Knockback
    public void TakeDamage(int damage, Vector2? knockbackDir = null, float? customForce = null)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (knockbackDir.HasValue && rb != null)
        {
            rb.velocity = Vector2.zero;
            Vector2 knockDir = knockbackDir.Value;

            if (Mathf.Abs(knockDir.x) > 0.7f) // ชนด้านข้าง
            {
                knockDir.y = 0.2f;
                knockDir.x = Mathf.Sign(knockDir.x);
            }
            else // โดนจากด้านบน/ใต้
            {
                knockDir.y = Mathf.Clamp(knockDir.y, 0.3f, 0.5f); // ลดสูง
            }

            float forceToUse = customForce.HasValue ? customForce.Value : knockbackForce;

            if (Mathf.Abs(knockDir.x) < 0.7f) // แนวตั้ง → ลดแรงรวม
                forceToUse *= 0.6f;

            rb.AddForce(knockDir.normalized * forceToUse, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    private void Die()
    {
        Debug.Log("Player Dead!");
        SceneManager.LoadScene(sceneName);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door")) // ต้องตั้ง Tag ของประตูเป็น "Door"
        {
            if (targetObject != null)
                targetObject.SetActive(true); // เปิด Object
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            if (targetObject != null)
                targetObject.SetActive(false); // ปิด Object
        }
    }



}
