using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // สำหรับ UI

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    public int maxHealth = 3;       // พลังชีวิตสูงสุด
    public int currentHealth;       // พลังชีวิตปัจจุบัน

    [Header("UI Elements")]
    public Slider healthBar;        // ลาก Slider จาก Canvas มาใส่ตรงนี้

    private void Start()
    {
        // ตั้งค่าเริ่มต้น
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    // เรียกเวลาผู้เล่นโดนโจมตี
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // เพิ่มเลือด (กรณีมีไอเท็มฟื้นพลัง)
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDir, float knockbackForce)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
            healthBar.value = currentHealth;

        // Knockback
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // reset ก่อน
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
            Die();
    }

    // ฟังก์ชันตาย
    void Die()
    {
        Debug.Log("Player Dead!");
        // ทำ Game Over, Restart scene หรือ Animation ตาย ได้ที่นี่
    }
}
