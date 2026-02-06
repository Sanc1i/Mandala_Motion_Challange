using System.Collections;
using UnityEngine;

public class MandalaController : MonoBehaviour
{
    [Header("Mandala Parts")]
    public Renderer[] mandalaRings;
    public Color baseColor = Color.cyan;
    public Color activeColor = Color.yellow;

    [Header("Audio Feedback")]
    public AudioSource narrationSource;
    public AudioSource sfxSource;
    public AudioClip motivationalClip;
    public AudioClip sectionCompleteClip;
    
    [Header("Systems")]
    public RhythmManager rhythmManager;
    public GamificationManager gamificationManager;

    // State Tracking
    private float _lastInputTime;
    private int _consecutiveHits = 0;
    private float _currentIntensity = 0f;
    private bool[] _sectionCompleted;
    private int _completedCount = 0;

    void Start()
    {
        _lastInputTime = Time.time;
        _sectionCompleted = new bool[mandalaRings.Length];
        
        // Subscribe to rhythm beat events
        rhythmManager.onBeatPulse.AddListener(OnBeatPulse);
    }

    void Update()
    {
        CheckIdleState();
        UpdateVisuals();
    }

    void OnBeatPulse()
    {
        // Play gentle pulse sound on each beat
        if (sfxSource != null && _currentIntensity > 0.2f)
        {
            sfxSource.volume = Mathf.Lerp(0.1f, 0.5f, _currentIntensity);
            sfxSource.PlayOneShot(rhythmManager.beatPulseClip);
        }
    }

    public void ProcessPlayerInput(bool isOnBeat)
    {
        _lastInputTime = Time.time;

        if (isOnBeat)
        {
            // Positive reinforcement
            _consecutiveHits++;
            _currentIntensity = Mathf.Clamp01(_currentIntensity + 0.1f);
            
            // Gradual tempo increase
            if (_consecutiveHits % 10 == 0 && rhythmManager.bpm < 75) 
            {
                rhythmManager.UpdateBPM(rhythmManager.bpm + 2);
            }

            // Check section completion
            CheckSectionProgress();
        }
        else
        {
            // Neutral feedback - don't punish, just slow growth
            _consecutiveHits = 0;
        }
    }

    void CheckSectionProgress()
    {
        // Simulate section progression based on consecutive hits
        int currentSection = Mathf.Min(_consecutiveHits / 20, mandalaRings.Length - 1);
        
        if (!_sectionCompleted[currentSection])
        {
            CompleteMandalaSection(currentSection);
        }
    }

    void CompleteMandalaSection(int sectionIndex)
    {
        _sectionCompleted[sectionIndex] = true;
        _completedCount++;

        // Play completion sound
        if (sfxSource != null && sectionCompleteClip != null)
        {
            sfxSource.PlayOneShot(sectionCompleteClip);
        }

        // Award points
        if (gamificationManager != null)
        {
            gamificationManager.AddPoints(100);
        }

        // Check if all sections complete
        if (_completedCount >= mandalaRings.Length)
        {
            OnAllSectionsComplete();
        }
        VirtualBuddy buddy = FindObjectOfType<VirtualBuddy>();
        if (buddy != null)
        {
           buddy.ReactToPlayerSuccess();
        }
    }

    void OnAllSectionsComplete()
    {
        Debug.Log("Mandala Master! All sections completed!");
        
        if (gamificationManager != null)
        {
            gamificationManager.AwardBadge("Mandala Master");
        }

        StartCoroutine(CompletionCelebration());
    }

    IEnumerator CompletionCelebration()
{
    // Play special completion music/sound
    if (sfxSource != null && sectionCompleteClip != null)
    {
        sfxSource.PlayOneShot(sectionCompleteClip);
    }

    // Cycle through rainbow colors on mandala
    float duration = 3f;
    float elapsed = 0f;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float hue = (elapsed / duration);
        Color rainbowColor = Color.HSVToRGB(hue, 0.8f, 1f);

        foreach (var ring in mandalaRings)
        {
            ring.material.SetColor("_EmissionColor", rainbowColor * 5f);
        }

        yield return null;
    }

    // Return to calm state
    _currentIntensity = 0.5f;
}

    void CheckIdleState()
    {
        // Idle detection
        if (Time.time - _lastInputTime > 5.0f)
        {
            // Gradually reduce intensity
            _currentIntensity = Mathf.Lerp(_currentIntensity, 0f, Time.deltaTime * 0.5f);
            
            // Slow down rhythm
            if (rhythmManager.bpm > 50)
            {
                rhythmManager.UpdateBPM(Mathf.Max(50, rhythmManager.bpm - Time.deltaTime * 0.5f));
            }

            // Play motivational narration (once per idle period)
            if (!narrationSource.isPlaying && motivationalClip != null && _currentIntensity < 0.1f)
            {
                narrationSource.PlayOneShot(motivationalClip);
            }
        }
    }

    void UpdateVisuals()
    {
        // Pulse based on rhythm
        float pulse = Mathf.Sin(Time.time * (rhythmManager.bpm / 60f) * Mathf.PI) * 0.5f + 0.5f;
        float finalEmission = pulse * (_currentIntensity * 3f);
        
        Color finalColor = Color.Lerp(baseColor, activeColor, _currentIntensity);

        for (int i = 0; i < mandalaRings.Length; i++)
        {
            // Brighter colors for completed sections
            Color ringColor = _sectionCompleted[i] ? 
                Color.Lerp(finalColor, Color.white, 0.3f) : finalColor;
            
            mandalaRings[i].material.SetColor("_EmissionColor", ringColor * finalEmission);
        }
    }

    void OnDestroy()
    {
        if (rhythmManager != null)
        {
            rhythmManager.onBeatPulse.RemoveListener(OnBeatPulse);
        }
    }
}
