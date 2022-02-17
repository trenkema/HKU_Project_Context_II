using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Map : MonoBehaviour
{
    [SerializeField] Transform player;

    [SerializeField] GameObject map;

    private void LateUpdate()
    {
        Vector3 newPlayerPosition = player.position;
        newPlayerPosition.y = transform.position.y;
        transform.position = newPlayerPosition;
    }

    public void ToggleMap(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            map.SetActive(!map.activeInHierarchy);

            EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, map.activeInHierarchy);
        }
    }
}
