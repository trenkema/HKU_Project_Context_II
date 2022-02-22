using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestObjectiveItem : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;

    string objective;

    public void SetObjective(string _objective)
    {
        objective = _objective;
    }

    public void UpdateAmount(int _currentAmount, int _maxAmount)
    {
        objectiveText.text = string.Format("[{0}/{1}] {2}", _currentAmount, _maxAmount, objective);
    }
}
