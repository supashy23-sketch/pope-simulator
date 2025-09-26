using UnityEngine;
using UnityEngine.Rendering.Universal;



public class PlayerLightController : MonoBehaviour
{
    [Header("Lights")]
    public Light2D globalLight;
    public Light2D playerLight;

    [Header("Charge Settings")]
    public float chargeTime = 3f;
    public int maxLight = 10;
    public int currentLight = 0;

    private float chargeCounter = 0f;
    private bool isCharging = false;
    private bool hasLight = false;

    [Header("Light Radii")]
    public float smallRadius = 1f;
    public float largeRadius = 5f;
    public float minGlobalIntensity = 0.1f;
    public float maxGlobalIntensity = 0.5f;

    private float lightFloat = 0f; // ใช้เก็บค่า float เพื่อลดทีละหน่วย

    void Start()
    {
        if (globalLight != null)
            globalLight.intensity = minGlobalIntensity;
        if (playerLight != null)
            playerLight.pointLightOuterRadius = 0f;
    }

    void Update()
    {
        // --- ชาร์จ ---
        if (Input.GetMouseButton(1))
        {
            isCharging = true;
            chargeCounter += Time.deltaTime;

            if (chargeCounter < chargeTime)
            {
                // ยังชาร์จอยู่ → แสงเล็ก
                if (playerLight != null)
                    playerLight.pointLightOuterRadius = smallRadius;
                if (globalLight != null)
                    globalLight.intensity = minGlobalIntensity + 0.2f;
            }
            else
            {
                // ชาร์จครบ → รีเซ็ตพลังใหม่ทุกครั้ง
                currentLight = maxLight;
                lightFloat = maxLight;
                hasLight = true;

                if (playerLight != null)
                    playerLight.pointLightOuterRadius = largeRadius;
                if (globalLight != null)
                    globalLight.intensity = maxGlobalIntensity;
            }
        }
        else
        {
            isCharging = false;
            chargeCounter = 0f;
        }

        // --- ใช้พลัง ---
        if (hasLight && lightFloat > 0f)
        {
            lightFloat -= Time.deltaTime;
            if (lightFloat < 0f) lightFloat = 0f;

            currentLight = Mathf.CeilToInt(lightFloat);

            if (playerLight != null)
                playerLight.pointLightOuterRadius = largeRadius;
            if (globalLight != null)
                globalLight.intensity = maxGlobalIntensity;
        }

        // --- หมดพลัง ---
        if (hasLight && currentLight <= 0)
        {
            hasLight = false;

            if (playerLight != null)
                playerLight.pointLightOuterRadius = 0f;
            if (globalLight != null)
                globalLight.intensity = minGlobalIntensity;
        }
    }
}