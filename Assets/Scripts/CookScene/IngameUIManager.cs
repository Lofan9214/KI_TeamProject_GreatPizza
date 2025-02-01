using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IngameUIManager : MonoBehaviour
{
    public ChatWindow chatWindow;
    public TextMeshProUGUI currencyText;

    public void ShowChatWindow(int[] Ids)
    {
        chatWindow.gameObject.SetActive(true);
        chatWindow.SetStrings(Ids);
    }

    public void UpdateCurrentCurrency()
    {
        currencyText.text = SaveLoadManager.Data.currency.ToString("F2");
    }
}
