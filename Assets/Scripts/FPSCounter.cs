using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText;

    private void Update()
    {
        float fps = 1f / Time.unscaledDeltaTime;
        fpsText.text = fps.ToString();
    }
}
