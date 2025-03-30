using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioMixer audiomixer;

    public const string MUSIC_KEY = "Music";
    public const string SOUND_KEY = "Sound";


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);


        }
        else
        {

            Destroy(gameObject);

        }
        LoadVolume();

    }

    void LoadVolume()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float soundVolume = PlayerPrefs.GetFloat(SOUND_KEY, 1f);

        audiomixer.SetFloat("Music", Mathf.Log10(musicVolume) * 20);
        audiomixer.SetFloat("Sound", Mathf.Log10(soundVolume) * 20);


    }
}