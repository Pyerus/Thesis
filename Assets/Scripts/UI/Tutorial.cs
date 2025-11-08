using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] dialogues;
    private float typingSpeed = 0.04f;
    [SerializeField] private AudioClip[] typingSounds;

    private int currentDialogueIndex = 0;
    private TMP_Text currentText;
    private Coroutine typingCoroutine;

    void Start()
    {
        foreach (var d in dialogues)
            d.SetActive(false);

        // Activate the first dialogue
        if (dialogues.Length > 0)
        {
            dialogues[0].SetActive(true);
            SetupDialogue(dialogues[0]);
        }
    }

    private void SetupDialogue(GameObject dialogue)
    {
        currentText = dialogue.GetComponentInChildren<TMP_Text>();

        if (currentText != null)
        {
            string fullText = currentText.text;
            currentText.text = "";
            typingCoroutine = StartCoroutine(TypeText(fullText));
        }

        Button nextButton = dialogue.GetComponentInChildren<Button>();
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(StartNextDialogue);
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        // Use AudioManager's SFX source (if available)
        AudioSource sfxSource = AudioManager.Instance != null ? AudioManager.Instance.sfxSource : null;

        foreach (char c in textToType)
        {
            currentText.text += c;

            // Play typing sound with correct SFX volume
            if (sfxSource != null && typingSounds.Length > 0)
            {
                AudioClip randomClip = typingSounds[Random.Range(0, typingSounds.Length)];
                sfxSource.PlayOneShot(randomClip, sfxSource.volume);
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void StartNextDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogues[currentDialogueIndex].SetActive(false);
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            dialogues[currentDialogueIndex].SetActive(true);
            SetupDialogue(dialogues[currentDialogueIndex]);
        }
        else
        {
            CircleTransition.Instance.TransitionToScene("MainMenuScene");
        }
    }
}
