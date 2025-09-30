using UnityEngine;

public class LightProjectile : MonoBehaviour
{
    public int damage = 5;
    public float lifeTime = 2f; // อยู่ 2 วินาทีแล้วหาย

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, Vector2.zero, 0f); // แรง knockback = 0
            
        }
    }
}
