using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeActions : MonoBehaviour
{
    public static FreezeActions Instance { get; private set; }

    public bool isFrozen { get; private set; }

    public bool isPositionFrozen { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventSystemNew<bool>.Subscribe(Event_Type.FREEZE_ACTIONS, SetFrozenState);
        EventSystemNew<bool>.Subscribe(Event_Type.FREEZE_POSITION, SetFrozenPosition);
    }

    private void OnDisable()
    {
        EventSystemNew<bool>.Unsubscribe(Event_Type.FREEZE_ACTIONS, SetFrozenState);
        EventSystemNew<bool>.Unsubscribe(Event_Type.FREEZE_POSITION, SetFrozenPosition);
    }

    private void SetFrozenState(bool _isFrozen)
    {
        isFrozen = _isFrozen;
    }

    private void SetFrozenPosition(bool _isFrozen)
    {
        isPositionFrozen = _isFrozen;
    }
}
