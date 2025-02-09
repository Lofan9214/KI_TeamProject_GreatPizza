using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class FormattedLocalizationText : LocalizationText
{
    public string[] unFormatedString;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            OnChangedLanguage(Variables.currentLanguage);
        }
#if UNITY_EDITOR
        else
        {
            OnChangedLanguage(editorLang);
        }
#endif
    }

    new public void OnChangedLanguage(Languages lang)
    {
        var stringTableId = DataTableIds.String[(int)lang];
        var stringTable = DataTableManager.Get<StringTable>(stringTableId);
        if (int.TryParse(stringId, out int id)
            && unFormatedString.Length > 0)
        {
            text.text = string.Format(stringTable.Get(id), unFormatedString);
        }
    }

    new public void SetString(string str)
    {
        unFormatedString = new string[] { str };
        OnChangedLanguage(Variables.currentLanguage);
    }

    public void SetString(string[] str)
    {
        unFormatedString = str;
        OnChangedLanguage(Variables.currentLanguage);
    }
}