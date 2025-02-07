using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    private Toggle toggle;
    public TextMeshProUGUI text;
    private const string fpsFormat = "{0:N1} FPS ({1:N1}ms)";

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OffFps);
    }

    private void Update()
    {
        if (toggle.isOn)
        {
            float fps = 1.0f / Time.deltaTime;
            float ms = Time.deltaTime * 1000.0f;
            text.text = string.Format(fpsFormat, fps, ms);
        }
    }

    private void OffFps(bool value)
    {
        if (!value)
        {
            text.text = "FPS Stat Off";
        }
    }
}
