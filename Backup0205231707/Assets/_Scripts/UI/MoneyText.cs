using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyText : MonoBehaviour
{
    int moneyCount;
    public void IncrementMoneyCount(float Value)
    {
        moneyCount += (int)Value;
        GetComponent<TextMeshProUGUI>().text = $"Money: {moneyCount}";
    }
}
