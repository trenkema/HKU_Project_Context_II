using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestItem : MonoBehaviour
{
    public TextMeshProUGUI questName;
    public TextMeshProUGUI questDescription;

    public TextMeshProUGUI progressionText;

    public void UpdateAmount(int _currentAmount, int _maxAmount)
    {
        progressionText.text = string.Format("{0}/{1}", _currentAmount, _maxAmount);
    }
}
