using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIManager : MonoBehaviour
{
    public FormattedLocalizationText startButtonText;
    public ShopWindow shopWindow;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    private void Start()
    {
        startButtonText.SetString((SaveLoadManager.Data.days+1).ToString());
    }

    public void NewGame()
    {
        SaveLoadManager.Data = new SaveDataV1();
        SaveLoadManager.Save();

        StartGame();
    }
}
