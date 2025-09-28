using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightController : MonoBehaviour
{
    [Header("Lights")]
    public Light2D playerLight;

    [Header("Charge Settings")]
    public int maxLight = 10;
    public int currentLight = 0;
    public float chargeTime = 3f;       // เวลากดขวาค้างเพื่อเริ่มชาร์จ
    public float lightDecayPerSecond = 1f; // ลดพลังต่อวินาที

    private float chargeCounter = 0f;
    private float decayCounter = 0f;

    [Header("Light Settings")]
    public float bigIntensity = 1.5f;
    public float bigRadius = 5f;
    public float smallIntensity = 0.3f;
    public float smallRadius = 1.5f;

    void Update()
    {
        HandleCharging();
        HandleDecay();
        UpdateLight();
    }

    void HandleCharging()
    {
        // กดคลิกขวาเพื่อชาร์จ
        if (Input.GetMouseButton(1))
        {
            chargeCounter += Time.deltaTime;

            if (chargeCounter >= chargeTime)
            {
                chargeCounter = 0f;
                if (currentLight < maxLight)
                    currentLight++;
            }
        }
        else
        {
            chargeCounter = 0f;
        }
    }

    void HandleDecay()
    {
        // ลดพลังงานทีละวินาที ถ้าไม่ได้ชาร์จ
        if (!Input.GetMouseButton(1) && currentLight > 0)
        {
            decayCounter += Time.deltaTime;
            if (decayCounter >= 1f)
            {
                currentLight -= (int)lightDecayPerSecond;
                if (currentLight < 0) currentLight = 0;
                decayCounter = 0f;
            }
        }
    }

    void UpdateLight()
    {
        if (currentLight > 0)
        {
            // มีพลังงาน → ไฟใหญ่
            playerLight.enabled = true;
            playerLight.intensity = bigIntensity;
            playerLight.pointLightOuterRadius = bigRadius;
        }
        else if (Input.GetMouseButton(1))
        {
            // กำลังชาร์จแต่ currentLight == 0 → ไฟเล็ก
            playerLight.enabled = true;
            playerLight.intensity = smallIntensity;
            playerLight.pointLightOuterRadius = smallRadius;
        }
        else
        {
            // ไม่มีพลังงานและไม่ได้ชาร์จ → ปิดไฟ
            playerLight.enabled = false;
        }
    }
}
