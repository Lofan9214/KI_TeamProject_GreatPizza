using System.Collections;
using TMPro;
using UnityEngine;

public class IngameUIManager : MonoBehaviour
{
    public ChatWindow chatWindow;
    public TextMeshProUGUI budgetText;
    public IngameGameManager gameManager;
    public TextMeshProUGUI tipText;

    public void ShowChatWindow(int[] Ids)
    {
        chatWindow.gameObject.SetActive(true);
        chatWindow.SetStrings(Ids);
    }

    public void UpdateCurrentBudget()
    {
        budgetText.text = gameManager.tempSaveData.budget.ToString("F2");
    }

    public void ShowTipMessage(float tip, float time)
    {
        StartCoroutine(ShowTipMessage(tip.ToString("F2"), time));
    }

    private  IEnumerator ShowTipMessage(string tip, float time)
    {
        tipText.text = tip;
        tipText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        tipText.gameObject.SetActive(false);
    }
}
