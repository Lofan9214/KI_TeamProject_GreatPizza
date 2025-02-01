using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizationText : MonoBehaviour
{
    public string stringId;

#if UNITY_EDITOR
    public Languages editorLang;
#endif

    protected TextMeshProUGUI text;

    protected void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    protected void OnEnable()
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
            text.text = stringTable.Get(id);
        }
    }

    public void SetString(string stringId)
    {
        this.stringId = stringId;
        OnEnable();
    }
}
