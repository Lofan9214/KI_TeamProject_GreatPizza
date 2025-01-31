using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUIManager : MonoBehaviour
{
    public ChatWindow chatWindow;

    public void ShowChatWindow(int[] Ids)
    {
        chatWindow.gameObject.SetActive(true);
        chatWindow.SetStrings(Ids);
    }
}
