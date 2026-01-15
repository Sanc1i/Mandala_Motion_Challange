using UnityEngine;
using UnityEngine.Events;

public class RhythmManager : MonoBehaviour
{
    [Header("Settings")]
    public float bpm = 60f; // Beats per minute
    public AudioSource backgroundMusic;
    
    [Header("Events")]
    public UnityEvent onBeatPulse; // Triggered every beat

    private float _beatTimer;
    private float _beatInterval;
    public bool IsBeatWindow { get; private set; } // Is it currently "on beat"?

    void Start()
    {
        UpdateBPM(bpm);
    }

    void Update()
    {
        _beatTimer += Time.deltaTime;

        // Check for beat
        if (_beatTimer >= _beatInterval)
        {
            _beatTimer -= _beatInterval;
            onBeatPulse.Invoke();
            StartCoroutine(BeatWindowRoutine());
        }
    }

    // Allows a small margin of error for the user to move "on beat"
    System.Collections.IEnumerator BeatWindowRoutine()
    {
        IsBeatWindow = true;
        yield return new WaitForSeconds(0.2f); // 200ms window to react
        IsBeatWindow = false;
    }

    public void UpdateBPM(float newBpm)
    {
        bpm = newBpm;
        _beatInterval = 60f / bpm;
        backgroundMusic.pitch = bpm / 60f; // Speed up music slightly with BPM
    }
}
