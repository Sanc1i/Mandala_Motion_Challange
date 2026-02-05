using UnityEngine;
using UnityEngine.Events;

public class OverwhelmDetector : MonoBehaviour
{
    [Header("Tuning")]
    public float jerkyThreshold = 15.0f;

    public float timeUntilOverwhelmed = 2.5f;

    public float recoveryTimeNeeded = 3.0f;

    [Header("Events")]
    public UnityEvent onOverwhelmDetected;
    public UnityEvent onCalmRestored;

    public bool IsOverwhelmed { get; private set; } = false;

    private float _jerkyTimer = 0f;
    private float _calmTimer = 0f;

    public void ProcessMotion(float intensity)
    {
        if (IsOverwhelmed)
        {
            MonitorRecovery(intensity);
        }
        else
        {
            MonitorStress(intensity);
        }
    }

    void MonitorStress(float intensity)
    {
        if (intensity > jerkyThreshold)
        {
            _jerkyTimer += Time.deltaTime;

            if (_jerkyTimer > timeUntilOverwhelmed)
            {
                TriggerOverwhelm();
            }
        }
        else
        {
            _jerkyTimer = Mathf.Max(0, _jerkyTimer - Time.deltaTime);
        }
    }

    void MonitorRecovery(float intensity)
    {
        if (intensity < (jerkyThreshold * 0.5f))
        {
            _calmTimer += Time.deltaTime;

            if (_calmTimer > recoveryTimeNeeded)
            {
                Recover();
            }
        }
        else
        {
            _calmTimer = 0f;
        }
    }

    void TriggerOverwhelm()
    {
        IsOverwhelmed = true;
        _calmTimer = 0f;
        Debug.Log("Overwhelm Triggered: Movement too violent.");
        onOverwhelmDetected.Invoke();
    }

    void Recover()
    {
        IsOverwhelmed = false;
        _jerkyTimer = 0f;
        Debug.Log("Calm Restored.");
        onCalmRestored.Invoke();
    }
}