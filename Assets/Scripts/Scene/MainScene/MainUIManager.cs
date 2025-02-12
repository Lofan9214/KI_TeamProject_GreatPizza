using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIManager : MonoBehaviour
{
    public GameObject newGameButton;
    public FormattedLocalizationText startButtonText;
    public ShopWindow shopWindow;

    private void Start()
    {
        if (SaveLoadManager.Data.days > 0)
        {
            newGameButton.SetActive(true);
        }
        startButtonText.SetString((SaveLoadManager.Data.days + 1).ToString());
    }
}
