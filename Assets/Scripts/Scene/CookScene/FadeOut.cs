using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    private readonly int closeHash = Animator.StringToHash("Close");
    public IngameTimeManager timeManager;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnFadeOut()
    {
        timeManager.endWindow.gameObject.SetActive(true);
    }

    public void PlayFadeOut()
    {
        animator.SetTrigger(closeHash);
    }
}
