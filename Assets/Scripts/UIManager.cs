using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Enums;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public static UIManager Singleton;

    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject levelSelection;
    [SerializeField] private GameObject counters;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private TextMeshProUGUI miceLeft;
    [SerializeField] private TextMeshProUGUI movesCounter;
    [SerializeField] private TextMeshProUGUI timeCounter;
    [SerializeField] private float fadingDuration = 5.0f;
    [SerializeField] private Transform levelsContent;
    [SerializeField] private GameObject placeholderPrefab;

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
    

    public Sprite outOfBounds;
    public Sprite wall;
    public Sprite empty;
    public Sprite hole;

    private Coroutine timeCoroutine;
    
    void Awake()
    {
        if (Singleton == null) Singleton = this;
        else return;
        menuScreen.SetActive(true);
        levelSelection.SetActive(true);
    }

    public void OnLevelClicked(Placeholder placeholder)
    {
        Manager.Singleton.SelectedLevel(placeholder.level);
        levelSelection.SetActive(false);
        Button();
    }

    public void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                StartCoroutine(FadingMenu(fadingDuration));
                break;
            case GameState.Playing:
                counters.SetActive(true);
                break;
            case GameState.Victory:
                counters.SetActive(false);
                victoryScreen.SetActive(true);
                StopCoroutine(timeCoroutine);
                soundEffects.clip = victory;
                soundEffects.Play();
                break;
        }
    }

    private IEnumerator FadingMenu(float duration)
    {
        CanvasGroup canvasGroup = menuScreen.GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(2.0f);
        backgroundMusic.clip = menu;
        backgroundMusic.Play();
        float time = 0;
        StartCoroutine(FadingLevels(duration*0.67f));
        while (time < duration)
        {
            float fraction = Mathf.Min(1, time / duration);
            canvasGroup.alpha = 1 - fraction;
            time += Time.deltaTime;
            yield return null;
        }
        menuScreen.SetActive(false);

        yield return null;
    }

    private IEnumerator FadingLevels(float wait)
    {
        yield return new WaitForSeconds(wait);
        Object[] objects = Resources.LoadAll("Levels/");
        TextAsset[] levels = new TextAsset[objects.Length];
        for (int i=0; i<objects.Length; i++)
            levels[i] = (TextAsset) objects[i];

        levels.OrderBy(level => int.Parse(level.name));
        foreach (TextAsset level in levels)
        {
            GameObject placeholder = Instantiate(placeholderPrefab, levelsContent);
            placeholder.GetComponent<Placeholder>().level = level;
            placeholder.GetComponent<Placeholder>().levelName.text = level.name;
        }
    }

    public void StartTimeCoroutine()
    {
        if (timeCoroutine == null)
            timeCoroutine = StartCoroutine(StartTime());
    }
    public IEnumerator StartTime()
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
        timeCounter.text = "00:00";
        int time = 0;
        while (true)
        {
            time += 1;
            float minutesFloat = time / 60.0f;
            int minutes = (int) minutesFloat;
            int seconds = time - minutes * 60;
            timeCounter.text = $"{minutes:000}:{seconds:00}s";
            yield return new WaitForSeconds(1.0f);
        }
    }
    
    public void UpdateMoves(int amount)
    {
        movesCounter.text = amount.ToString();
    }
    
    public void UpdateMouse(int amount)
    {
        miceLeft.text = amount.ToString();
    }

    public void Reset()
    {
        movesCounter.text = "0";
        miceLeft.text = "0";
        soundEffects.clip = reset;
        soundEffects.Play();
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
    
}
