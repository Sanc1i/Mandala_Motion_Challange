using UnityEngine;
using System.Collections;

public class VirtualBuddy : MonoBehaviour
{
    [Header("Appearance")]
    public Renderer buddyRenderer;
    public Color idleColor = Color.blue;
    public Color activeColor = Color.green;
    public Color encouragingColor = Color.yellow;

    [Header("Animation")]
    public float bobSpeed = 1f;
    public float bobHeight = 0.2f;
    
    [Header("Pointing")]
    public Transform mandalaTarget;
    public float pointingInterval = 10f; // Point every 10 seconds

    private Vector3 _startPosition;
    private bool _isPointing = false;
    private MandalaController _mandalaController;

    void Start()
    {
        _startPosition = transform.position;
        _mandalaController = FindObjectOfType<MandalaController>();
        
        StartCoroutine(IdleBobbing());
        StartCoroutine(PeriodicPointing());
    }

    IEnumerator IdleBobbing()
    {
        while (true)
        {
            if (!_isPointing)
            {
                float newY = _startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
            }
            yield return null;
        }
    }

    IEnumerator PeriodicPointing()
    {
        while (true)
        {
            yield return new WaitForSeconds(pointingInterval);
            
            if (mandalaTarget != null)
            {
                yield return StartCoroutine(PointAtMandala());
            }
        }
    }

    IEnumerator PointAtMandala()
    {
        _isPointing = true;
        
        // Change color to encouraging
        SetBuddyColor(encouragingColor);
        
        // Look at mandala
        Vector3 direction = mandalaTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        float rotationTime = 0.5f;
        Quaternion startRotation = transform.rotation;
        float elapsed = 0f;
        
        while (elapsed < rotationTime)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / rotationTime);
            yield return null;
        }
        
        // Hold pointing for 2 seconds
        yield return new WaitForSeconds(2f);
        
        // Return to idle
        SetBuddyColor(idleColor);
        _isPointing = false;
    }

    public void ReactToPlayerSuccess()
    {
        StartCoroutine(CelebrationPulse());
    }

    IEnumerator CelebrationPulse()
    {
        // Pulse green 3 times
        for (int i = 0; i < 3; i++)
        {
            SetBuddyColor(activeColor);
            yield return new WaitForSeconds(0.2f);
            SetBuddyColor(idleColor);
            yield return new WaitForSeconds(0.2f);
        }
    }

    void SetBuddyColor(Color color)
    {
        if (buddyRenderer != null)
        {
            buddyRenderer.material.SetColor("_EmissionColor", color * 2f);
        }
    }

    public void ShowEncouragement()
    {
        SetBuddyColor(encouragingColor);
        // Could add particle effect or animation here
    }
}
