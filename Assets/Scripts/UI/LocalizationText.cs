using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizationText : MonoBehaviour
{
    public string stringId;
    public bool formatted;
    public string stringFormat;
    private string unFormatedString;

#if UNITY_EDITOR
    public Languages editorLang;
#endif

    private TextMeshProUGUI text;


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

    public void OnChangedLanguage(Languages lang)
    {
        var stringTableId = DataTableIds.String[(int)lang];
        var stringTable = DataTableManager.Get<StringTable>(stringTableId);
        if (int.TryParse(stringId, out int id))
        {
            if (formatted)
            {
                text.text = string.Format(stringFormat, stringTable.Get(id), unFormatedString);
            }
            else
            {
                text.text = stringTable.Get(id);
            }
        }
    }

    public void OnSetString(string str)
    {
        unFormatedString = str;
        OnChangedLanguage(Variables.currentLanguage);
    }
}
