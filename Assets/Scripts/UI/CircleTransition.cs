using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CircleTransition : MonoBehaviour
{
    public static CircleTransition Instance;
    public Image circleImage;
    public float transitionSpeed = 2f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(DoTransition(sceneName));
    }

    private IEnumerator DoTransition(string sceneName)
    {
        // Close circle
        yield return StartCoroutine(FillCircle(1f));

        // Load next scene
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FillCircle(float target)
    {
        float start = circleImage.fillAmount;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            circleImage.fillAmount = Mathf.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }

    // Call this from new scene’s Start() if you want it to open up again
    public void OpenFromBlack()
    {
        StartCoroutine(FillCircle(0f));
    }
}
