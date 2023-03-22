using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio clips")]
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource soundEffects;

    [SerializeField] private AudioClip menu;
    [SerializeField] private AudioClip level1;
    [SerializeField] private AudioClip level2;
    [SerializeField] private AudioClip level3;
    [SerializeField] private AudioClip level4;
    [SerializeField] private AudioClip level5;
    [SerializeField] private AudioClip button;
    [SerializeField] private AudioClip start;
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioClip push;

    [SerializeField] private AudioClip reset;
    private void Start()
    {
        if (Instance == null) Instance = this;
        else return;
    }


    public void Push()
    {
        soundEffects.clip = push;
        soundEffects.Play();
    }

    public void Button()
    {
        soundEffects.clip = button;
        soundEffects.Play();
    }

    public void Reset()
    {
        soundEffects.clip = reset;
        soundEffects.Play();
    }

    public void Victory()
    {
        soundEffects.clip = victory;
        soundEffects.Play();
    }

    public void StartMusic()
    {
        int random = Random.Range(1, 5);
        switch (random)
        {
            case 1:
                backgroundMusic.clip = level1;
                break;
            case 2:
                backgroundMusic.clip = level2;
                break;
            case 3:
                backgroundMusic.clip = level3;
                break;
            case 4:
                backgroundMusic.clip = level4;
                break;
            case 5:
                backgroundMusic.clip = level5;
                break;
        }
        backgroundMusic.Play();
        soundEffects.clip = start;
        soundEffects.Play();
    }

    public void StartMenuMusic()
    {
        backgroundMusic.clip = menu;
        backgroundMusic.Play();
    }


}
