using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class IngredientToggler : MonoBehaviour
{
    private Toggle toggle;
    public TextMeshProUGUI text;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    public void Init(string name, bool value)
    {
        text.text = name;
        toggle.isOn = value;
        UnityAction<bool> func = val =>
        {
            if (SaveLoadManager.Data.unlocks.ContainsKey(name))
            {
                SaveLoadManager.Data.unlocks[name] = val;
            }
        };
        toggle.onValueChanged.AddListener(func);
    }
}
