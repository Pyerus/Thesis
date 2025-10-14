using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public float hoverScale = 1.1f;   // how big it gets on hover
    public float transitionSpeed = 8f; // how fast it animates
    public Color hoverColor = Color.white; // optional tint

    private Vector3 originalScale;
    private Color originalColor;
    private bool isHovered = false;

    private Image image;

    private void Start()
    {
        originalScale = transform.localScale;
        image = GetComponent<Image>();
        if (image != null)
            originalColor = image.color;
    }

    private void Update()
    {
        // Smooth scale animation
        Vector3 targetScale = isHovered ? originalScale * hoverScale : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);

        // Optional color tint
        if (image != null)
        {
            Color targetColor = isHovered ? hoverColor : originalColor;
            image.color = Color.Lerp(image.color, targetColor, Time.deltaTime * transitionSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}
