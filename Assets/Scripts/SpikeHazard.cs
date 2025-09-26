using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpikeHazard : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;
    
    public float globalHitCooldown = 0.5f;

    [Header("Knockback")]
    
    public float knockbackForce = 7.5f;
    [Range(0f, 1f)] public float upBias = 0.9f;
    [Range(0f, 1f)] public float sideBias = 0.35f;

    [Header("Anti-stuck")]
    
    public float popUpEject = 0.04f;

    
    private static readonly Dictionary<int, float> s_nextHitTimeByPlayer = new Dictionary<int, float>();

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true; 
    }

    private void OnTriggerEnter2D(Collider2D other) => TryHitImmediate(other);

   
    private void OnTriggerStay2D(Collider2D other) => TryHitImmediate(other);

    private void TryHitImmediate(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        int playerId = other.GetInstanceID();
        float now = Time.time;
        if (s_nextHitTimeByPlayer.TryGetValue(playerId, out float next) && now < next)
            return; 

    
        var hp = other.GetComponent<PlayerHealth>();
        if (hp != null) hp.TakeDamage(damage);

  
        var rb = other.attachedRigidbody ?? other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
         
            rb.position += Vector2.up * popUpEject;

            
            float sideDir = Mathf.Sign(other.transform.position.x - transform.position.x);
            Vector2 dir = (Vector2.up * upBias) + (Vector2.right * sideDir * sideBias);
            if (dir.sqrMagnitude < 1e-4f) dir = Vector2.up;

           
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, 0f));
            rb.AddForce(dir.normalized * knockbackForce, ForceMode2D.Impulse);
        }

      
        s_nextHitTimeByPlayer[playerId] = now + globalHitCooldown;
    }
}
