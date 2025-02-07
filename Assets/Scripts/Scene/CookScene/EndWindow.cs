using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndWindow : MonoBehaviour
{
    private IngameGameManager gameManager;

    public TextMeshProUGUI dayName;
    public TextMeshProUGUI dayStart;
    public TextMeshProUGUI dayEnd;

    public void Show()
    {
        gameObject.SetActive(true);
        gameManager ??= GameObject.FindGameObjectWithTag("GameController").GetComponent<IngameGameManager>();
        dayName.text = $"Days : {gameManager.tempSaveData.days}";
        dayStart.text = $"day start : {SaveLoadManager.Data.budget:F2}";
        dayEnd.text = $"day end : {gameManager.tempSaveData.budget:F2}";
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
