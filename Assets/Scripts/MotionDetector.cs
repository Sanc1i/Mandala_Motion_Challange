using UnityEngine;
using UnityEngine.XR;

public class MotionDetector : MonoBehaviour
{
    [Header("References")]
    public RhythmManager rhythmManager;
    public MandalaController mandalaController; // Reference to the script below

    [Header("Sensitivity")]
    public float movementThreshold = 0.5f; // How fast they must move
    
    private Vector3 _lastPosition;
    private float _currentSpeed;

   private OverwhelmDetector _overwhelmDetector;

    void Start()
    {
        _overwhelmDetector = FindObjectOfType<OverwhelmDetector>();
    }

    void Update()
    {
        // 1. Calculate velocity ONCE
        Vector3 velocity = (transform.position - _lastPosition) / Time.deltaTime;
        _currentSpeed = velocity.magnitude;
        _lastPosition = transform.position; // Update last position after calc

        // 2. Report to OverwhelmDetector
        if (_overwhelmDetector != null && _currentSpeed > 0.1f)
        {
            _overwhelmDetector.RecordMovement(velocity);
        }

        // 3. Check Threshold logic
        if (_currentSpeed > movementThreshold)
        {
            if (rhythmManager.IsBeatWindow)
            {
                mandalaController.ProcessPlayerInput(true);
            }
            else
            {
                mandalaController.ProcessPlayerInput(false);
            }
        }
    }
}
