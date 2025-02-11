using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIManager : MonoBehaviour
{
    public GameObject newGameButton;
    public FormattedLocalizationText startButtonText;
    public ShopWindow shopWindow;

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    private void Start()
    {
        if (SaveLoadManager.Data.days > 0)
        {
            newGameButton.SetActive(true);
        }
        startButtonText.SetString((SaveLoadManager.Data.days + 1).ToString());
    }

    public void NewGame()
    {
        SaveLoadManager.Data = new SaveDataV2();
        SaveLoadManager.Save();

        StartGame();
    }
}
