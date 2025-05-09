using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class SoundsVariety
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1;

    public bool loop = false;

    public bool randomPitch = false;

    [Range(0.1f, 3f)]
    public float pitch = 1;
}
