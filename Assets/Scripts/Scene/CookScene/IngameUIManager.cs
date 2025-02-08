using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIManager : MonoBehaviour
{
    public ChatWindow chatWindow;
    public TextMeshProUGUI budgetText;
    public IngameGameManager gameManager;
    public TextMeshProUGUI tipText;
    public OrderWindow orderWindow;
    public Button orderShowButton;
    public GameObject tutorialWindow;
    public LocalizationText tutorialText;

    public void ShowChatWindow(int[] Ids, bool story = false)
    {
        chatWindow.gameObject.SetActive(true);
        if (story)
        {
            chatWindow.SetStoryStrings(Ids);
            return;
        }
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

    private IEnumerator ShowTipMessage(string tip, float time)
    {
        tipText.text = tip;
        tipText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        tipText.gameObject.SetActive(false);
    }

    public void OnOrderButtonClick()
    {
        orderWindow.gameObject.SetActive(true);
        orderWindow.Init(chatWindow.stringIds, chatWindow.talkIndex);
    }

    public void SetOrderButtonActive(bool active)
    {
        orderShowButton.gameObject.SetActive(active);
    }
}
