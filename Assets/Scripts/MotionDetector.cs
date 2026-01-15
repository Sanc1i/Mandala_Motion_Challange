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

    void Update()
    {
        // Calculate velocity manually (more reliable across SDKs)
        _currentSpeed = (transform.position - _lastPosition).magnitude / Time.deltaTime;
        _lastPosition = transform.position;

        // LOGIC: If moving fast enough AND inside the rhythm window
        if (_currentSpeed > movementThreshold)
        {
            if (rhythmManager.IsBeatWindow)
            {
                // Perfect hit
                mandalaController.ProcessPlayerInput(true);
            }
            else
            {
                // Moving off-beat (Neutral/Negative feedback logic)
                mandalaController.ProcessPlayerInput(false);
            }
        }
    }
}
