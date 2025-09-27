using UnityEngine;
using TMPro;

public class ItemCounter : MonoBehaviour
{
    [Header("Assign all number texts in order")]
    public TextMeshProUGUI[] countTexts; // drag all the "0" texts here
    private int[] counts;

    void Start()
    {
        counts = new int[countTexts.Length];
        UpdateAllTexts();
    }

    public void Increase(int index)
    {
        counts[index]++;
        UpdateText(index);
    }

    public void Decrease(int index)
    {
        if (counts[index] > 0)
        {
            counts[index]--;
            UpdateText(index);
        }
    }

    public void Buy(int index)
    {
        // later: handle inventory, money, etc.
        counts[index] = 0;
        UpdateText(index);
    }

    private void UpdateText(int index)
    {
        countTexts[index].text = counts[index].ToString();
    }

    private void UpdateAllTexts()
    {
        for (int i = 0; i < countTexts.Length; i++)
        {
            countTexts[i].text = counts[i].ToString();
        }
    }
}
