using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleFlicker : MonoBehaviour
{
    public Light2D candleLight;
    public float minIntensity = 0.4f;
    public float maxIntensity = 0.7f;
    public float flickerSpeed = 10f;

    void Update()
    {
        if (candleLight != null)
        {
            candleLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(Time.time * flickerSpeed, 0));
        }
    }
}

