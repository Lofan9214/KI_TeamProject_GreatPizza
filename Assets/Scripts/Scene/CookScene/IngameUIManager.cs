using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUIManager : MonoBehaviour
{
    private readonly int tipAppearHash = Animator.StringToHash("TipAppear");
    private readonly int refundAppearHash = Animator.StringToHash("RefundAppear");
    public ChatWindow chatWindow;
    public TextMeshProUGUI budgetText;
    public IngameGameManager gameManager;
    public TextMeshProUGUI tipText;
    public Animator tipAnimator;
    public OrderWindow orderWindow;
    public Button orderShowButton;
    public GameObject tutorialWindow;
    public LocalizationText tutorialText;
    public GameObject tutorialArrow;
    public Satisafaction satisafaction;

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

    public IEnumerator ShowTipMessage(float tip, float time)
    {
        tipText.text = tip.ToString("F2");
        tipText.transform.parent.gameObject.SetActive(true);
        if (tip < 0f)
            tipAnimator.SetTrigger(refundAppearHash);
        else
            tipAnimator.SetTrigger(tipAppearHash);
        yield return new WaitForSeconds(1f);
        tipText.transform.parent.gameObject.SetActive(false);
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
