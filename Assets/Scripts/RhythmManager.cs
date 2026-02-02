using UnityEngine;
using UnityEngine.Events;

public class RhythmManager : MonoBehaviour
{
    [Header("Settings")]
    public float bpm = 60f;
    public AudioSource backgroundMusic;
    
    [Header("Audio Clips")]
    public AudioClip beatPulseClip;
    
    [Header("Events")]
    public UnityEvent onBeatPulse;

    private float _beatTimer;
    private float _beatInterval;
    public bool IsBeatWindow { get; private set; }

    void Start()
    {
        UpdateBPM(bpm);
        
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
    }

    void Update()
    {
        _beatTimer += Time.deltaTime;

        if (_beatTimer >= _beatInterval)
        {
            _beatTimer -= _beatInterval;
            onBeatPulse.Invoke();
            StartCoroutine(BeatWindowRoutine());
        }
    }

    System.Collections.IEnumerator BeatWindowRoutine()
    {
        IsBeatWindow = true;
        yield return new WaitForSeconds(0.25f); // 250ms window
        IsBeatWindow = false;
    }

    public void UpdateBPM(float newBpm)
    {
        bpm = Mathf.Clamp(newBpm, 50f, 120f);
        _beatInterval = 60f / bpm;
        
        // Adjust background music pitch slightly (subtle tempo change)
        if (backgroundMusic != null)
        {
            backgroundMusic.pitch = Mathf.Lerp(0.95f, 1.1f, (bpm - 50f) / 70f);
        }
    }
}
