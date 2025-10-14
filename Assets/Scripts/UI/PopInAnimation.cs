using UnityEngine;
using System.Collections;

public class PopInAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float startDelay = 0f;         // delay before animation starts
    public float duration = 0.5f;         // how long the pop-in lasts
    public float overshootScale = 1.2f;   // how big it grows before bouncing back
    public AnimationCurve curve;          // easing curve for bounce

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero; // start invisible (scaled down)
    }

    private void OnEnable()
    {
        StartCoroutine(PopIn());
    }

    private IEnumerator PopIn()
    {
        yield return new WaitForSeconds(startDelay);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Easing bounce effect
            float scaleFactor = curve.Evaluate(t) * overshootScale;
            transform.localScale = originalScale * scaleFactor;

            yield return null;
        }

        transform.localScale = originalScale; // ensure final scale is exact
    }
}
