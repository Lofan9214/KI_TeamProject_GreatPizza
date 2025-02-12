using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float timer = 0f;
    private int counter = 0;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        ++counter;
        if (timer > 1f)
        {
            text.text = counter.ToString();
            timer = 0f;
            counter = 0;
        }
    }
}
