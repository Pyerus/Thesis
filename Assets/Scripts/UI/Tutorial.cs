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

    private string fullText; 
    private bool isTyping = false;

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
            fullText = currentText.text;
            currentText.text = "";
            typingCoroutine = StartCoroutine(TypeText(fullText));
        }

        Button nextButton = dialogue.GetComponentInChildren<Button>();
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(OnClickDialogue);
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        // Use AudioManager's SFX source (if available)
        AudioSource sfxSource = AudioManager.Instance != null ? AudioManager.Instance.sfxSource : null;

        if (sfxSource != null && typingSounds.Length > 0)
        {
            AudioClip randomClip = typingSounds[Random.Range(0, typingSounds.Length)];
            sfxSource.PlayOneShot(randomClip, sfxSource.volume);
        }
        foreach (char c in textToType)
        {
            currentText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
    }

    private void OnClickDialogue()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            currentText.text = fullText;
            isTyping = false;
        }
        else
        {
            StartNextDialogue();
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
            Debug.Log("Teleporting to MainMenuScene");
        }
    }


    /////////////////////////////////////////////////////////////////////
    // BUG FIX ----------------------------------------------------------
    /////////////////////////////////////////////////////////////////////

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
