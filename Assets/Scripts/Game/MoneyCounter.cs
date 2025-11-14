using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textCounter;
    [SerializeField] float money = 100000f;

    private void Update()
    {
        textCounter.text = "$" + money;
    }

    public void AddMoney(float amount)
    {
        money += amount;
    }

    public void SubtractMoney(float amount)
    {
        money -= amount;
    }
}
