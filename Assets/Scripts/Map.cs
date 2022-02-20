using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Map : MonoBehaviour
{
    [SerializeField] Transform player;

    private void LateUpdate()
    {
        Vector3 newPlayerPosition = player.position;
        newPlayerPosition.y = transform.position.y;
        transform.position = newPlayerPosition;
    }
}
