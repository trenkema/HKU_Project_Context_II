using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFaceCamera : MonoBehaviour
{
    Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (cam != null)
        {
            Vector3 targetPosition = new Vector3(cam.transform.position.x, transform.position.y, cam.transform.position.z);
            transform.LookAt(targetPosition);
            transform.Rotate(Vector3.up * 180);
        }
        else
        {
            cam = Camera.main;
        }
    }
}
