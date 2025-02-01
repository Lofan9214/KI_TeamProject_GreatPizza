using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        dayStart.text = $"day start : {SaveLoadManager.Data.currency:F2}";
        dayEnd.text = $"day end : {gameManager.tempSaveData.currency:F2}";
    }

    public void Exit()
    {
        SaveLoadManager.Data.Set(gameManager.tempSaveData);
        SaveLoadManager.Save();
        SceneManager.LoadScene(0);
    }
}
