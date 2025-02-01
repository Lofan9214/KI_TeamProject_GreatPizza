using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMeshProUGUI))]
public class FormattedLocalizationText : LocalizationText
{
    public string stringFormat;
    private string unFormatedString;

    new public void OnChangedLanguage(Languages lang)
    {
        var stringTableId = DataTableIds.String[(int)lang];
        var stringTable = DataTableManager.Get<StringTable>(stringTableId);
        if (int.TryParse(stringId, out int id))
        {
            text.text = string.Format(stringFormat, stringTable.Get(id), unFormatedString);
        }
    }

    new public void SetString(string str)
    {
        unFormatedString = str;
        OnChangedLanguage(Variables.currentLanguage);
    }
}