using UnityEngine;
using System.Collections;

public class PopInAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float startDelay = 0f;         // delay before animation starts
    public float duration = 0.5f;         // how long the pop-in lasts
    public float overshootScale = 1.2f;   // how big it grows before bouncing back
    public AnimationCurve curve;          // easing curve for bounce

    private Vector3 targetScale;          // the scale we want to reach
    private bool hasInitialized = false;

    private void Awake()
    {
        // Save the original scale from the inspector
        if (!hasInitialized)
        {
            targetScale = transform.localScale;
            
            // Safety check: if scale is too small, use a default
            if (targetScale.magnitude < 0.1f)
            {
                targetScale = Vector3.one;
                Debug.LogWarning($"{gameObject.name} - Scale was too small ({transform.localScale}), using Vector3.one");
            }
            
            hasInitialized = true;
            Debug.Log($"{gameObject.name} - Target scale saved in Awake: {targetScale}, current scale: {transform.localScale}");
        }
    }

    private void OnEnable()
    {
        Debug.Log($"{gameObject.name} - OnEnable called at time: {Time.time}");
        
        // Make sure we have a valid target scale
        if (!hasInitialized)
        {
            targetScale = transform.localScale;
            hasInitialized = true;
            Debug.Log($"{gameObject.name} - Target scale saved in OnEnable: {targetScale}");
        }
        
        // Start the pop-in animation every time the object is enabled
        StopAllCoroutines(); // Stop any existing animations
        StartCoroutine(PopIn());
    }
    
    private void OnDisable()
    {
        Debug.Log($"{gameObject.name} - OnDisable called at time: {Time.time}");
    }

    private IEnumerator PopIn()
    {
        // Start at zero scale
        transform.localScale = Vector3.zero;
        
        yield return new WaitForSeconds(startDelay);

        Debug.Log($"{gameObject.name} - PopIn animation starting, target: {targetScale}");
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Use curve if set, otherwise use simple easing
            float scaleFactor;
            if (curve != null && curve.keys.Length > 0)
            {
                scaleFactor = curve.Evaluate(t);
            }
            else
            {
                // Default bounce easing: overshoot then settle
                if (t < 0.6f)
                {
                    scaleFactor = Mathf.Lerp(0, overshootScale, t / 0.6f);
                }
                else
                {
                    scaleFactor = Mathf.Lerp(overshootScale, 1f, (t - 0.6f) / 0.4f);
                }
            }
            
            transform.localScale = targetScale * scaleFactor;

            yield return null;
        }

        // STAY at target scale
        transform.localScale = targetScale;
        Debug.Log($"{gameObject.name} - PopIn complete, FINAL scale: {transform.localScale}");
    }
}