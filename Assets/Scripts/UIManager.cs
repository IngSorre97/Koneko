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
    [SerializeField] private GameObject levelSelectionPanel;
    [SerializeField] private GameObject levelSelectionContent;
    [SerializeField] private GameObject counters;
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private TextMeshProUGUI miceLeft;
    [SerializeField] private TextMeshProUGUI movesCounter;
    [SerializeField] private TextMeshProUGUI timeCounter;
    [SerializeField] private TextMeshProUGUI gameStats;
    [SerializeField] private float fadingDuration = 5.0f;
    [SerializeField] private Transform levelsContent;
    [SerializeField] private GameObject placeholderPrefab;

    [SerializeField] private GameObject creditsScreen;

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
    [SerializeField] private AudioClip voice;
    [SerializeField] private AudioClip start;
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioClip push;
    [SerializeField] private AudioClip reset;
    private TextAsset[] levels;

    private Dictionary<TextAsset, Placeholder> _levelPlaceholders = new Dictionary<TextAsset, Placeholder>();

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
    }

    public void OnLevelClicked(Placeholder placeholder)
    {
        Manager.Singleton.SelectedLevel(placeholder.level);
        levelSelectionPanel.SetActive(false);
        
        Button();
    }

    public void OnCreditClicked()
    {
        creditsScreen.SetActive(!creditsScreen.activeSelf);
        Button();
    }

    public void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                levelSelectionPanel.SetActive(true);
                if (levels == null)
                {
                    StartCoroutine(FadingMenu(fadingDuration));
                    levelSelectionContent.SetActive(false);
                }
                else
                {
                    SetRecords();
                }
                
                victoryScreen.SetActive(false);
                break;
            case GameState.Playing:
                counters.SetActive(true);
                levelName.text = Manager.Singleton._currentLevel.name;
                victoryScreen.SetActive(false);
                break;
            case GameState.Victory:
                counters.SetActive(false);
                victoryScreen.SetActive(true);
                StopCoroutine(timeCoroutine);
                timeCoroutine = null;
                gameStats.text = $"Moves: {Manager.Singleton.GetMoves()}   Time:{timeCounter.text}";
                soundEffects.clip = victory;
                soundEffects.Play();
                break;
        }
    }

    private IEnumerator FadingMenu(float duration)
    {
        CanvasGroup canvasGroup = menuScreen.GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(0.5f);
        soundEffects.clip = button;
        soundEffects.Play();
        yield return new WaitForSeconds(0.5f);
        //soundEffects.clip = voice;
        //soundEffects.Play();
        yield return new WaitForSeconds(1.0f);
        backgroundMusic.clip = menu;
        backgroundMusic.Play();
        float time = 0;
        
        StartCoroutine(FadingLevels(duration*0.90f));
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

    private void SetRecords()
    {
        foreach(TextAsset lvl in levels)
        {
            _levelPlaceholders[lvl].SetRecordString( RecordManager.Singleton.getRecordString(lvl));
        }
    }

    private IEnumerator FadingLevels(float wait)
    {
        yield return new WaitForSeconds(wait);

        levelSelectionContent.SetActive(true);
        if (levels == null)
        {
            Object[] objects = Resources.LoadAll("Levels/");
            levels = new TextAsset[objects.Length];
            for (int i = 0; i < objects.Length; i++)
                levels[i] = (TextAsset)objects[i];

            levels.OrderBy(level => level.name);
            foreach (TextAsset level in levels)
            {
                GameObject placeholder = Instantiate(placeholderPrefab, levelsContent);
                
                placeholder.GetComponent<Placeholder>().level = level;
                placeholder.GetComponent<Placeholder>().levelName.text = level.name;
                _levelPlaceholders[level] = placeholder.GetComponent<Placeholder>();
            }
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
            timeCounter.text = $"{minutes:000}m:{seconds:00}s";
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

    public void Reset(bool nextLevel)
    {
        movesCounter.text = "0";
        miceLeft.text = "0";
        
        if (nextLevel )
        {
            timeCounter.text = "00:00";
            if(timeCoroutine != null)
            {
                StopCoroutine(timeCoroutine);
                timeCoroutine = null;
            }
            
        }
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


    public void OnClickNextLevel()
    {
        victoryScreen.SetActive(false);
        int i;
        for (i=0;i < levels.Length;i++)
        {
            if(levels[i] == Manager.Singleton._currentLevel){
                break;
            }
        }
        Manager.Singleton.SelectedLevel(levels[(i + 1) % levels.Length]);

    }

    public string GetTime()
    {
        return timeCounter.text;
    }

}
