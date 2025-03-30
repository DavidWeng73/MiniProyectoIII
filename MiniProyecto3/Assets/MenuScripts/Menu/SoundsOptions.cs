using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundOptions : MonoBehaviour
{
    [SerializeField] AudioMixer mainMixer;

    [SerializeField] Slider sliderSound;
    [SerializeField] Slider sliderMusic;





    private void Awake()
    {
        sliderMusic.onValueChanged.AddListener(setmasterVolume);
        sliderSound.onValueChanged.AddListener(setFXVolume);
    }

    private void Start()
    {
        sliderMusic.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, 1f);
        sliderSound.value = PlayerPrefs.GetFloat(AudioManager.SOUND_KEY, 1f);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(AudioManager.MUSIC_KEY, sliderMusic.value);
        PlayerPrefs.SetFloat(AudioManager.SOUND_KEY, sliderSound.value);

    }

    void setmasterVolume(float sliderValue)
    {

        mainMixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
    }

    void setFXVolume(float sliderValue)
    {
        mainMixer.SetFloat("Sound", Mathf.Log10(sliderValue) * 20);
    }


}