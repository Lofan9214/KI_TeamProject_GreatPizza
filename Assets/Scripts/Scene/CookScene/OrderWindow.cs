using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrderWindow : MonoBehaviour
{
    public TextMeshProUGUI[] texts;

    public void Init(int[] stringIds, ChatWindow.Talks talks)
    {
        for (int i = 0; i < texts.Length; ++i)
        {
            texts[i].text = DataTableManager.StringTable.Get(stringIds[i]);
            texts[i].gameObject.SetActive(i <= (int)talks);
        }
        if ((talks == ChatWindow.Talks.Hint2)
            && (stringIds[1] == 111002 || stringIds[1] == 111006))
        {
            texts[1].gameObject.SetActive(false);
        }
    }
}
