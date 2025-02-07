using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OrderWindow : MonoBehaviour, IPointerDownHandler
{
    public TextMeshProUGUI[] texts;

    public void Init(int[] stringIds, ChatWindow.Talks talks)
    {
        for (int i = 0; i < texts.Length; ++i)
        {
            texts[i].text = DataTableManager.StringTable.Get(stringIds[i]);
            texts[i].enabled = i <= (int)talks;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }
}
