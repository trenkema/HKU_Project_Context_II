using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class MapManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;

    [SerializeField] GameObject map;

    [SerializeField] Camera mapCamera;

    [SerializeField] TextMeshProUGUI followPlayerText;
    [SerializeField] Image followPlayerButton;

    [SerializeField] GameObject mouseMoveText;
    [SerializeField] GameObject mouseMoveImage;

    [Header("Settings")]
    [SerializeField] float zoomSpeed = 1f;

    [SerializeField] float maxZoomHeight = 300f;

    [SerializeField] float minZoomHeight = 100f;

    [SerializeField] float mapMinX, mapMaxX, mapMinY, mapMaxY;

    [SerializeField] Color followingColor, UnfollowingColor;

    Vector3 dragOrigin;

    float mouseScrollY = 0f;

    bool canDrag = false;

    bool followingPlayer = true;

    private void Awake()
    {
        mouseScrollY = mapCamera.orthographicSize;

        mouseMoveText.SetActive(false);
        mouseMoveImage.SetActive(false);
    }

    private void LateUpdate()
    {
        if (followingPlayer)
        {
            Vector3 newPlayerPosition = player.position;
            newPlayerPosition.y = mapCamera.transform.position.y;

            mapCamera.gameObject.transform.position = ClampCamera(newPlayerPosition);
        }
    }

    public void ToggleMap(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            mapCamera.gameObject.SetActive(!mapCamera.gameObject.activeInHierarchy);

            map.SetActive(!map.activeInHierarchy);

            EventSystemNew<bool>.RaiseEvent(Event_Type.CURSOR_ON, map.activeInHierarchy);
            EventSystemNew<bool>.RaiseEvent(Event_Type.FREEZE_ACTIONS, map.activeInHierarchy);
        }
    }

    public void Zoom(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && map.activeInHierarchy)
        {
            if (context.ReadValue<float>() > 0)
                mouseScrollY -= zoomSpeed;
            else
                mouseScrollY += zoomSpeed;

            mouseScrollY = Mathf.Clamp(mouseScrollY, minZoomHeight, maxZoomHeight);

            mapCamera.orthographicSize = mouseScrollY;

            mapCamera.gameObject.transform.position = ClampCamera(mapCamera.gameObject.transform.position);
        }
    }

    public void PanClickedCamera(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && map.activeInHierarchy)
        {
            canDrag = true;

            dragOrigin = mapCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
        else if (context.phase == InputActionPhase.Canceled && map.activeInHierarchy)
        {
            canDrag = false;
        }
    }

    public void PanCamera(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && map.activeInHierarchy && canDrag)
        {
            Vector3 difference = dragOrigin - mapCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            mapCamera.gameObject.transform.position = ClampCamera(mapCamera.gameObject.transform.position + difference);
        }
    }

    public void ToggleFollowPlayer()
    {
        followingPlayer = !followingPlayer;

        if (followingPlayer)
        {
            followPlayerText.color = followingColor;
            followPlayerButton.color = followingColor;
            followPlayerText.text = "Following Player";

            mouseMoveText.SetActive(false);
            mouseMoveImage.SetActive(false);
        }
        else
        {
            followPlayerText.color = UnfollowingColor;
            followPlayerButton.color = UnfollowingColor;
            followPlayerText.text = "<s>Following Player</s>";

            mouseMoveText.SetActive(true);
            mouseMoveImage.SetActive(true);
        }
    }

    private Vector3 ClampCamera(Vector3 _targetPosition)
    {
        float camHeight = mapCamera.orthographicSize;
        float camWidth = mapCamera.orthographicSize * mapCamera.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minZ = mapMinY + camHeight;
        float maxZ = mapMaxY - camHeight;

        float newX = Mathf.Clamp(_targetPosition.x, minX, maxX);
        float newZ = Mathf.Clamp(_targetPosition.z, minZ, maxZ);

        return new Vector3(newX, _targetPosition.y, newZ);
    }
}
