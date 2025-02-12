using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public IngameTimeManager timeManager;

    public void OnFadeOut()
    {
        timeManager.endWindow.gameObject.SetActive(true);
    }
}
