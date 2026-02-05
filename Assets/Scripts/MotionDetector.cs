using UnityEngine;
using UnityEngine.XR;

public class MotionDetector : MonoBehaviour
{
    [Header("References")]
    public RhythmManager rhythmManager;
    public MandalaController mandalaController;
    public OverwhelmDetector overwhelmDetector;

    [Header("Tuning (Agitation Meter)")]
    public float activeThreshold = 2.0f;

    private Vector3 _lastLocalPos;
    private Quaternion _lastLocalRot;
    private float _currentIntensity;

    void Start()
    {
        _lastLocalPos = transform.localPosition;
        _lastLocalRot = transform.localRotation;

        if (overwhelmDetector == null)
            overwhelmDetector = FindObjectOfType<OverwhelmDetector>();
    }

    void Update()
    {
        float linearSpeed = Vector3.Distance(transform.localPosition, _lastLocalPos) / Time.deltaTime;

        float angleChange = Quaternion.Angle(transform.localRotation, _lastLocalRot);
        float angularSpeed = angleChange / Time.deltaTime;

        _currentIntensity = (linearSpeed * 2.0f) + (angularSpeed * 0.1f);

        _lastLocalPos = transform.localPosition;
        _lastLocalRot = transform.localRotation;

        if (overwhelmDetector != null)
        {
            overwhelmDetector.ProcessMotion(_currentIntensity);

            if (overwhelmDetector.IsOverwhelmed) return;
        }

        if (_currentIntensity > activeThreshold)
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