using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    public AudioMixer audioMixer;

    private const string bgmVol = "bgmVol";
    private const string sfxVol = "sfxVol";

    private void Start()
    {
        audioMixer.GetFloat(bgmVol, out float bgmvol);
        bgmSlider.value = Mathf.Pow(10, bgmvol * 0.05f);
        audioMixer.GetFloat(sfxVol, out float sfxvol);
        sfxSlider.value = Mathf.Pow(10, sfxvol * 0.05f);

        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void SetBgmVolume(float vol)
    {
        audioMixer.SetFloat(bgmVol, Mathf.Log10(vol) * 20f);
    }
    private void SetSfxVolume(float vol)
    {
        audioMixer.SetFloat(sfxVol, Mathf.Log10(vol) * 20f);
    }
}
