using UnityEngine;
using UnityEngine.Events;

public class OverwhelmDetector : MonoBehaviour
{
    [Header("Detection Parameters")]
    public float rapidMovementThreshold = 3f;    // movements per second
    public float pauseThreshold = 8f;            // seconds of no movement
    public float erraticMovementThreshold = 0.8f; // variance in movement direction
    
    [Header("Events")]
    public UnityEvent onOverwhelmDetected;
    public UnityEvent onCalmRestored;

    private bool _isOverwhelmed = false;
    private float _lastMovementTime;
    private float _pauseDuration;
    private int _movementCount;
    private float _movementTimer;
    private Vector3[] _recentMovements = new Vector3[10];
    private int _movementIndex = 0;

    void Update()
    {
        DetectPauses();
        DetectErraticMovement();
        ResetMovementCounter();
    }

    public void RecordMovement(Vector3 velocity)
    {
        _lastMovementTime = Time.time;
        _movementCount++;
        
        // Store recent movement directions
        _recentMovements[_movementIndex] = velocity.normalized;
        _movementIndex = (_movementIndex + 1) % _recentMovements.Length;
    }

    void DetectPauses()
    {
        _pauseDuration = Time.time - _lastMovementTime;

        if (_pauseDuration > pauseThreshold && !_isOverwhelmed)
        {
            TriggerOverwhelm("Extended pause detected");
        }
    }

    void DetectErraticMovement()
    {
        // Calculate variance in movement directions
        float variance = CalculateDirectionVariance();

        if (variance > erraticMovementThreshold && !_isOverwhelmed)
        {
            TriggerOverwhelm("Erratic movement detected");
        }
    }

    void ResetMovementCounter()
    {
        _movementTimer += Time.deltaTime;
        
        if (_movementTimer >= 1f)
        {
            // Check movements per second
            if (_movementCount > rapidMovementThreshold * 1f && !_isOverwhelmed)
            {
                TriggerOverwhelm("Rapid movement detected");
            }

            _movementCount = 0;
            _movementTimer = 0f;
        }
    }

    float CalculateDirectionVariance()
    {
        if (_recentMovements[0] == Vector3.zero) return 0f;

        float totalVariance = 0f;
        Vector3 avgDirection = Vector3.zero;

        // Calculate average direction
        foreach (var movement in _recentMovements)
        {
            avgDirection += movement;
        }
        avgDirection /= _recentMovements.Length;

        // Calculate variance from average
        foreach (var movement in _recentMovements)
        {
            totalVariance += Vector3.Distance(movement, avgDirection);
        }

        return totalVariance / _recentMovements.Length;
    }

    void TriggerOverwhelm(string reason)
    {
        _isOverwhelmed = true;
        Debug.Log($"Overwhelm detected: {reason}");
        
        onOverwhelmDetected.Invoke();

        // Start recovery monitoring
        StartCoroutine(MonitorRecovery());
    }

    System.Collections.IEnumerator MonitorRecovery()
    {
        // Wait for calm, steady movement
        float calmTime = 0f;
        float requiredCalmDuration = 5f;

        while (calmTime < requiredCalmDuration)
        {
            yield return new WaitForSeconds(1f);

            // Check if movement is calm
            float variance = CalculateDirectionVariance();
            if (variance < erraticMovementThreshold * 0.5f && _movementCount < rapidMovementThreshold * 0.5f)
            {
                calmTime += 1f;
            }
            else
            {
                calmTime = 0f; // Reset if not calm
            }
        }

        // Calm restored
        _isOverwhelmed = false;
        Debug.Log("Calm restored");
        onCalmRestored.Invoke();
    }
}
