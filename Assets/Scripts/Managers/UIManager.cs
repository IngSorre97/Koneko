using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Enums;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public class UIManager : MonoBehaviour
{
    public class LevelFile
    {
        public string name;
        public TextAsset level;
    }

    public static UIManager Singleton;

    [SerializeField] private TextMeshProUGUI introduction;
    [Header("Parameters")]
    [SerializeField] private float fadingDuration = 2.0f;
    [SerializeField] private List<string> levelsPages = new List<string>();
    
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
    [SerializeField] private TextMeshProUGUI victoryMoves;
    [SerializeField] private TextMeshProUGUI victoryTime;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject bottomButtons;

    [Header("Sprites")]
    public Sprite outOfBounds;
    public Sprite wall;
    public Sprite empty;
    public Sprite hole;

    private int _maxPages;
    private int _currentPage;
    private int _currentLevel;
    private List<List<LevelFile>> _levels = null;
    private Dictionary<LevelFile, Placeholder> _levelPlaceholders = new Dictionary<LevelFile, Placeholder>();
    private Coroutine timeCoroutine;
    
    void Awake()
    {
        if (Singleton == null) Singleton = this;
        else return;
        TurnOffPanels();

        titlePanel.SetActive(true);
        menuPanel.SetActive(true);
    }

    private void TurnOffPanels()
    {
        menuPanel.SetActive(false);
        countersPanel.SetActive(false);
        bottomButtons.SetActive(false);
        victoryPanel.SetActive(false);
    }

    public void UpdateState(GameState state)
    {
        TurnOffPanels();
        switch (state)
        {
            case GameState.Menu:
                if (_levels == null)
                {
                    menuPanel.SetActive(true);
                    introductionScreen.SetActive(true);
                    StartCoroutine(FadingMenu());
                    GenerateLevels();
                }
                else
                {
                    menuPanel.SetActive(true);
                    SetRecords();
                    LoadLevels(_currentPage);
                }

                break;
            case GameState.Playing:
                countersPanel.SetActive(true);
                bottomButtons.SetActive(true);
                levelName.text = Manager.Singleton._currentLevel.name;
                break;
            case GameState.Victory:
                victoryPanel.SetActive(true);
                StopCoroutine(timeCoroutine);
                timeCoroutine = null;
                victoryMoves.text = Manager.Singleton.GetMoves().ToString();
                victoryTime.text = timeCounter.text;
                SoundManager.Instance.Victory();
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
        SoundManager.Instance.Button();
    }
    
    public void OnPlayClicked()
    {
        introductionScreen.SetActive(false);
        LoadLevels(_currentPage);
        SoundManager.Instance.Button();
    }
    
    public void OnLevelClicked(Placeholder placeholder)
    {
        Manager.Singleton.SelectedLevel(placeholder.level);
        menuPanel.SetActive(false);
        levelSelectionScreen.SetActive(false);
        _currentLevel = placeholder.index;

        SoundManager.Instance.Button();
    }


    
    //******************************************************************************************************************
    //*** Coroutines
    
    private IEnumerator FadingMenu()
    {
        CanvasGroup canvasGroup = titlePanel.GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(1.0f);
        SoundManager.Instance.Button();
        yield return new WaitForSeconds(1.0f);
        SoundManager.Instance.StartMenuMusic();

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

        int counter = 0;
        foreach (LevelFile level in _levels[index])
        {
            GameObject placeholder = Instantiate(placeholderPrefab, levelsContent);

            placeholder.GetComponent<Placeholder>().index = counter;
            placeholder.GetComponent<Placeholder>().level = level;
            placeholder.GetComponent<Placeholder>().levelName.text = level.name;
            _levelPlaceholders[level] = placeholder.GetComponent<Placeholder>();
            counter++;
        }
    }

    private void GenerateLevels()
    {
        int current = 0;
        _levels = new List<List<LevelFile>>();
        foreach(string page in levelsPages)
        {
            _levels.Add(new List<LevelFile>());
            foreach (Object level in Resources.LoadAll("Levels/" + page + "/"))
            {
                LevelFile levelFile = new LevelFile();
                levelFile.name = level.name;
                levelFile.level = (TextAsset) level;
                _levels[current].Add(levelFile);
            }
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
        SoundManager.Instance.StartMusic();
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
        _currentLevel = Mathf.Min(_currentLevel + 1, _levels[_currentPage].Count - 1);
        Manager.Singleton.SelectedLevel(_levels[_currentPage][_currentLevel]);

    }

    public void OnClickRetry()
    {
        victoryPanel.SetActive(false);
        Manager.Singleton.SelectedLevel(_levels[_currentPage][_currentLevel]);

    }

    public void OnClickNextPage()
    {
        SoundManager.Instance.Button();
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
        SoundManager.Instance.Button();
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
        SoundManager.Instance.Reset();
    }

    public void OnZoomOutClicked()
    {
        Manager.Singleton.OnZoomOutClicked();
    }

    public void OnZoomInClicked()
    {
        Manager.Singleton.OnZoomInClicked();
    }

    public string GetTime()
    {
        return timeCounter.text;
    }

}
