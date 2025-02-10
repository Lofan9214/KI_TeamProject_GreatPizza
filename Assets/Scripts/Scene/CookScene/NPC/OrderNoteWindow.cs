using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderNoteWindow : MonoBehaviour
{
    public GameObject window;
    public GameObject orderButton;

    public void CloseWindow()
    {
        window.SetActive(false);
        orderButton.SetActive(true);
    }

}
