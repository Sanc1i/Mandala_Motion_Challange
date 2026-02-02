using UnityEngine;

public class VisualFeedbackManager : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem beatPulseEffect;
    public ParticleSystem sectionCompleteEffect;
    public ParticleSystem overwhelmedEffect;

    [Header("Light Beams")]
    public Light[] rhythmLights;
    public float lightPulseIntensity = 2f;
    public Color onBeatColor = Color.cyan;
    public Color offBeatColor = Color.blue;

    private RhythmManager _rhythmManager;

    void Start()
    {
        _rhythmManager = FindObjectOfType<RhythmManager>();
        
        if (_rhythmManager != null)
        {
            _rhythmManager.onBeatPulse.AddListener(OnBeat);
        }

        // Setup rhythm lights
        SetupRhythmLights();
    }

    void SetupRhythmLights()
    {
        // Create light beams around mandala
        for (int i = 0; i < rhythmLights.Length; i++)
        {
            if (rhythmLights[i] == null)
            {
                GameObject lightObj = new GameObject($"RhythmLight_{i}");
                lightObj.transform.parent = transform;
                
                // Position in circle around mandala
                float angle = (360f / rhythmLights.Length) * i;
                float radius = 2f;
                lightObj.transform.position = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                    1.5f,
                    3f + Mathf.Sin(angle * Mathf.Deg2Rad) * radius
                );
                
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Spot;
                light.range = 5f;
                light.spotAngle = 30f;
                light.intensity = 0f;
                light.color = onBeatColor;
                
                // Point at mandala
                lightObj.transform.LookAt(new Vector3(0, 1.5f, 3f));
                
                rhythmLights[i] = light;
            }
        }
    }

    void OnBeat()
    {
        // Pulse beat effect
        if (beatPulseEffect != null)
        {
            beatPulseEffect.Play();
        }

        // Pulse lights
        StartCoroutine(PulseLights());
    }

    System.Collections.IEnumerator PulseLights()
    {
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float intensity = Mathf.Lerp(lightPulseIntensity, 0f, t);

            foreach (Light light in rhythmLights)
            {
                if (light != null)
                {
                    light.intensity = intensity;
                }
            }

            yield return null;
        }

        // Ensure lights are off
        foreach (Light light in rhythmLights)
        {
            if (light != null)
            {
                light.intensity = 0f;
            }
        }
    }

    public void PlaySectionComplete(Vector3 position)
    {
        if (sectionCompleteEffect != null)
        {
            sectionCompleteEffect.transform.position = position;
            sectionCompleteEffect.Play();
        }
    }

    public void ShowOverwhelmedFeedback()
    {
        if (overwhelmedEffect != null)
        {
            overwhelmedEffect.Play();
        }
        
        // Dim all rhythm lights to blue
        foreach (Light light in rhythmLights)
        {
            if (light != null)
            {
                light.color = offBeatColor;
                light.intensity = 0.5f;
            }
        }
    }

    void OnDestroy()
    {
        if (_rhythmManager != null)
        {
            _rhythmManager.onBeatPulse.RemoveListener(OnBeat);
        }
    }


public void ActivateQuietZone()
{
    // Dim all lights
    foreach (Light light in rhythmLights)
    {
        if (light != null)
        {
            light.color = new Color(0.3f, 0.3f, 0.6f); // Soft blue
            light.intensity = 0.3f;
        }
    }

    // Could show UI hint: "Take your time. Move slowly when ready."
    Debug.Log("Quiet zone activated - giving player space");
}

public void DeactivateQuietZone()
{
    // Restore normal light colors
    foreach (Light light in rhythmLights)
    {
        if (light != null)
        {
            light.color = onBeatColor;
        }
    }
}
}
