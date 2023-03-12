using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Enums;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public static UIManager Singleton;

    [Header("Parameters")]
    [SerializeField] private float fadingDuration = 2.0f;
    
    [Header("Menu canvases")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject introductionScreen;
    [SerializeField] private GameObject levelSelectionScreen;
    [SerializeField] private GameObject creditsScreen;
    
    
    
    
    [SerializeField] private Transform levelsContent;
    [SerializeField] private GameObject placeholderPrefab;

    
    
    [Header("Gameplay canvases")]
    [SerializeField] private GameObject countersPanel;
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private TextMeshProUGUI miceLeft;
    [SerializeField] private TextMeshProUGUI movesCounter;
    [SerializeField] private TextMeshProUGUI timeCounter;
    [SerializeField] private TextMeshProUGUI gameStats;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button nextLevel;
    
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
    
    [Header("Sprites")]
    public Sprite outOfBounds;
    public Sprite wall;
    public Sprite empty;
    public Sprite hole;

    private int _maxPages;
    private int _currentPage;
    private int _currentLevel;
    private List<List<TextAsset>> _levels = null;
    private Dictionary<TextAsset, Placeholder> _levelPlaceholders = new Dictionary<TextAsset, Placeholder>();
    private Coroutine timeCoroutine;
    
    void Awake()
    {
        if (Singleton == null) Singleton = this;
        else return;
        titlePanel.SetActive(true);
        menuPanel.SetActive(true);
    }
    
    public void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                if (_levels == null)
                {
                    StartCoroutine(FadingMenu());
                    GenerateLevels();
                }
                else
                {
                    victoryPanel.SetActive(false);
                    menuPanel.SetActive(true);
                    SetRecords();
                    LoadLevels(_currentPage);
                }

                break;
            case GameState.Playing:
                countersPanel.SetActive(true);
                levelName.text = Manager.Singleton._currentLevel.name;
                victoryPanel.SetActive(false);
                break;
            case GameState.Victory:
                countersPanel.SetActive(false);
                victoryPanel.SetActive(true);
                if (_currentLevel == _levels[_currentPage].Count)
                    nextLevel.gameObject.SetActive(false);
                StopCoroutine(timeCoroutine);
                timeCoroutine = null;
                gameStats.text = $"Moves: {Manager.Singleton.GetMoves()}   Time:{timeCounter.text}";
                soundEffects.clip = victory;
                soundEffects.Play();
                break;
        }
    }

    private void SetRecords()
    {
        /*
        foreach(TextAsset lvl in levels)
        {
            _levelPlaceholders[lvl].SetRecordString( RecordManager.Singleton.getRecordString(lvl));
        } */
    }

    //******************************************************************************************************************
    //*** Button functions
    public void OnCreditClicked()
    {
        creditsScreen.SetActive(!creditsScreen.activeSelf);
        Button();
    }
    
    public void OnPlayClicked()
    {
        introductionScreen.SetActive(false);
        LoadLevels(_currentPage);
        Button();
    }
    
    public void OnLevelClicked(Placeholder placeholder)
    {
        Manager.Singleton.SelectedLevel(placeholder.level);
        menuPanel.SetActive(false);
        levelSelectionScreen.SetActive(false);
        
        Button();
    }
    
    //******************************************************************************************************************
    //*** Coroutines
    
    private IEnumerator FadingMenu()
    {
        CanvasGroup canvasGroup = titlePanel.GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(1.0f);
        soundEffects.clip = button;
        soundEffects.Play();
        yield return new WaitForSeconds(1.0f);
        backgroundMusic.clip = menu;
        backgroundMusic.Play();

        introductionScreen.SetActive(true);
        
        float time = 0;
        while (time < fadingDuration)
        {
            float fraction = Mathf.Min(1, time / fadingDuration);
            canvasGroup.alpha = 1 - fraction;
            time += Time.deltaTime;
            yield return null;
        }
        
        titlePanel.SetActive(false);
        yield return null;
    }

    private void LoadLevels(int index)
    {
        levelSelectionScreen.SetActive(true);
        
        foreach(Transform child in levelsContent)
            Destroy(child.gameObject);
        
        foreach (TextAsset level in _levels[index])
        {
            GameObject placeholder = Instantiate(placeholderPrefab, levelsContent);
                
            placeholder.GetComponent<Placeholder>().level = level;
            placeholder.GetComponent<Placeholder>().levelName.text = level.name;
            _levelPlaceholders[level] = placeholder.GetComponent<Placeholder>();
        }
    }

    private void GenerateLevels()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Levels/");
        _levels = new List<List<TextAsset>>();
        int current = 0;
        foreach(var dir in dirInfo.GetDirectories())
        {
            _levels.Add(new List<TextAsset>());
            string[] subfolders = dir.Name.Split("/");
            string folderPath = subfolders[^1];
            Object[] levels = Resources.LoadAll("Levels/" + folderPath + "/");
            foreach (var textAsset in levels)
            {
                _levels[current].Add((TextAsset)textAsset);
            }
            _levels[current].OrderBy(level => level.name);
            current++;
        }
        _maxPages = current;
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
    
    public void OnClickNextLevel()
    {
        victoryPanel.SetActive(false);
        Manager.Singleton.SelectedLevel(_levels[_currentPage][_currentLevel+1]);
    }

    public void OnClickNextPage()
    {
        Button();
        _currentPage++;
        if (_currentPage > _maxPages - 1)
        {
            _currentPage--;
            return;
        }
        LoadLevels(_currentPage);

    }
    
    public void OnClickPreviousPage()
    {
        Button();
        _currentPage--;
        if (_currentPage < 0)
        {
            _currentPage++;
            return;
        }
        LoadLevels(_currentPage);
        
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
    
    public string GetTime()
    {
        return timeCounter.text;
    }

}
