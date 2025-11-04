using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] dialogues;
    private int currentDialogueIndex = 0;

    void Start()
    {
        foreach (var d in dialogues)
            d.SetActive(false);

        // Activate the first dialogue
        if (dialogues.Length > 0)
        {
            dialogues[0].SetActive(true);

            // Find the button in the first dialogue
            Button nextButton = dialogues[0].GetComponentInChildren<Button>();
            if (nextButton != null)
                nextButton.onClick.AddListener(StartNextDialogue);
        }
    }

public void StartNextDialogue()
    {
        dialogues[currentDialogueIndex].SetActive(false);

        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Length)
        {
            dialogues[currentDialogueIndex].SetActive(true);

            // Update the button listener for the new dialogue
            Button nextButton = dialogues[currentDialogueIndex].GetComponentInChildren<Button>();
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(StartNextDialogue);
            }
        }
        else
        {
            Debug.Log("Tutorial finished!");
        }
    }
}
