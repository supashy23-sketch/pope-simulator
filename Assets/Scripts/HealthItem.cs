using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [Header("Health Item Settings")]
    public int healAmount = 1; // ฟื้นพลังเท่าไหร่

    [Header("Optional")]
    public AudioClip pickupSound; // เสียงเก็บไอเท็ม
    public GameObject pickupEffect; // เอฟเฟกต์อนิเมชันสั้น ๆ

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบชน Player
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // เพิ่มเลือด
            playerHealth.Heal(healAmount);

            // เล่นเสียง (ถ้ามี)
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // สร้างเอฟเฟกต์ (ถ้ามี)
            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            // ทำลายไอเท็ม
            Destroy(gameObject);
        }
    }
}
