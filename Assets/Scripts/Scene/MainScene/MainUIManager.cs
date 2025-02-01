using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIManager : MonoBehaviour
{
    public Transform ShopWindow;



    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenShop()
    {
        ShopWindow.gameObject.SetActive(true);
    }

    public void CloseShop()
    {
        ShopWindow.gameObject.SetActive(false);
    }
}
