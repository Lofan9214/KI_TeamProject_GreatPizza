using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AutoCutter : MonoBehaviour
{
    private readonly int playHash = Animator.StringToHash("Play");
    private AudioSource audioSource;
    private Animator animator;
    public AudioClip clip;

    public CuttingSlot currentTable;
    public Cutter cutter;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        currentTable.CurrentPizza.Movable = false;
        gameObject.SetActive(true);
        animator.SetTrigger(playHash);
    }

    public void PlayCutSound()
    {
        audioSource.PlayOneShot(clip);
    }

    public void Cut()
    {
        currentTable.CurrentPizza.Cut(Quaternion.Euler(0f, 0f, 90f));
        currentTable.CurrentPizza.Cut(Quaternion.Euler(0f, 0f, 150f));
        currentTable.CurrentPizza.Cut(Quaternion.Euler(0f, 0f, 210f));
        currentTable.CurrentPizza.Movable = true;
        currentTable.CurrentPizza.CircleCollider.enabled = true;
        cutter.spriteRenderer.enabled = true;
        gameObject.SetActive(false);
    }
}
