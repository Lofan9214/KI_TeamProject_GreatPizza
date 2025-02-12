using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameSoundManager : MonoBehaviour
{
    public AudioClip dayEnd;
    public AudioClip dayEndBell;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDayEnd()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(dayEnd);
    }

    public void PlayEndBell()
    {
        audioSource.PlayOneShot(dayEndBell);
    }
}
