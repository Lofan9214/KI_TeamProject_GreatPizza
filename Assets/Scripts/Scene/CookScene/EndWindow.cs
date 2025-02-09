using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndWindow : MonoBehaviour
{
    private IngameGameManager gameManager;
    public FormattedLocalizationText dateText;
    public FormattedLocalizationText watchdateText;

    public TextMeshProUGUI totalProfitText;
    public TextMeshProUGUI tipText;
    public TextMeshProUGUI refundText;
    public TextMeshProUGUI ingredientUsageText;
    public TextMeshProUGUI netProfitText;
    public TextMeshProUGUI budgetText;


    private void Start()
    {
        gameManager ??= GameObject.FindGameObjectWithTag("GameController").GetComponent<IngameGameManager>();
        dateText.SetString(gameManager.tempSaveData.days.ToString());
        watchdateText.SetString(gameManager.tempSaveData.days.ToString());
        totalProfitText.text = gameManager.totalProfit.ToString("F2");
        tipText.text = gameManager.tip.ToString("F2");
        refundText.text = gameManager.refund.ToString("F2");
        ingredientUsageText.text = gameManager.ingredientUsage.ToString("F2");
        netProfitText.text = (gameManager.tempSaveData.budget - SaveLoadManager.Data.budget).ToString("F2");
        budgetText.text = gameManager.tempSaveData.budget.ToString("F2");
    }

    public void Exit()
    {
        if (gameManager.tempSaveData.budget >= 0f)
        {
            SaveLoadManager.Data.Set(gameManager.tempSaveData);
            SaveLoadManager.Save();
        }
        SceneManager.LoadScene(0);
    }
}
