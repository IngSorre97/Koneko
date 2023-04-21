using System.Collections;
using System.Collections.Generic;
using Enums;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

public class UIManager : MonoBehaviour
{
    public static UIManager Singleton;
    public bool doNotShowTutorial = false;

    [SerializeField] private TMP_InputField nicknameIF;
    public string nickname;

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
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject bottomButtons;

    private int _maxPages;
    private int _currentPage;
    private int _currentLevel;
    public int _currentID => _currentLevel;
    private List<List<Level>> _levels = null;
    private Dictionary<Level, Placeholder> _levelPlaceholders = new Dictionary<Level, Placeholder>();
    private Coroutine timeCoroutine;

    void Awake()
    {
        if (Singleton == null) Singleton = this;
        else return;
        TurnOffPanels();

    }

    public void StartGame()
    {
        titlePanel.SetActive(true);
        menuPanel.SetActive(true);
        introductionScreen.SetActive(true);
        StartCoroutine(FadingMenu());
    }

    private void TurnOffPanels()
    {
        menuPanel.SetActive(false);
        countersPanel.SetActive(false);
        bottomButtons.SetActive(false);
        victoryPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void UpdateState(GameState state)
    {
        TurnOffPanels();
        switch (state)
        {
            case GameState.Menu:
                menuPanel.SetActive(true);
                StartCoroutine(LoadLevels(_currentPage));
                break;
            case GameState.Playing:
                countersPanel.SetActive(true);
                tutorialPanel.SetActive(true);
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

    //******************************************************************************************************************
    //*** Button functions
    public void OnCreditClicked()
    {
        creditsScreen.SetActive(!creditsScreen.activeSelf);
        SoundManager.Instance.Button();
    }
    
    public void OnPlayClicked()
    {
        if (nicknameIF.text == "") return;
        nickname = nicknameIF.text;

        RecordManager.Singleton.SetRecords(nickname);
        introductionScreen.SetActive(false);
        StartCoroutine(GenerateLevels());
        SoundManager.Instance.Button();
    }
    
    public void OnLevelClicked(Placeholder placeholder)
    {
        Manager.Singleton.SelectedLevel(placeholder.level);
        if (placeholder.level.tutorial != Tutorials.None && !doNotShowTutorial)
            Instantiate(TutorialManager.Singleton.GetTutorial(placeholder.level.tutorial), tutorialPanel.transform);
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

    private IEnumerator LoadLevels(int index)
    {
        while (RecordManager.Singleton.isDirty)
            yield return new WaitForSeconds(0.25f);

        levelSelectionScreen.SetActive(true);
        
        foreach(Transform child in levelsContent)
            Destroy(child.gameObject);

        int counter = 0 +index*11; //TODO DA CAMBIARE
        foreach (Level level in _levels[index])
        {
            GameObject placeholder = Instantiate(placeholderPrefab, levelsContent);

            placeholder.GetComponent<Placeholder>().index = counter;
            placeholder.GetComponent<Placeholder>().level = level;
            placeholder.GetComponent<Placeholder>().levelName.text = level.menuName;

            int record = RecordManager.Singleton.GetRecordByLevel(counter.ToString());
            if (record != -1)
                Debug.Log("Found " + record + " for level " + counter.ToString());
            if (record == -1)
                placeholder.GetComponent<Placeholder>().stars.sprite = SpritesData.Singleton.noStars;
            else if (record < level.threeStars)
                placeholder.GetComponent<Placeholder>().stars.sprite = SpritesData.Singleton.threeStar;
            else if (record < level.twoStars)
                placeholder.GetComponent<Placeholder>().stars.sprite = SpritesData.Singleton.twoStar;
            else
                placeholder.GetComponent<Placeholder>().stars.sprite = SpritesData.Singleton.oneStar;


            _levelPlaceholders[level] = placeholder.GetComponent<Placeholder>();
            counter++;
        }

        yield return null;
    }

    private IEnumerator GenerateLevels()
    {
        while (RecordManager.Singleton.isDirty)
            yield return new WaitForSeconds(0.25f);

        int current = 0;
        _levels = new List<List<Level>>();
        foreach(string page in levelsPages)
        {
            _levels.Add(new List<Level>());
            foreach (Object level in Resources.LoadAll<Level>("Levels/" + page + "/"))
            {
                _levels[current].Add(level as Level);
            }
            current++;
        }
        _maxPages = current;
        StartCoroutine(LoadLevels(0));
        yield return null;
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
        StartCoroutine(LoadLevels(_currentPage));

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
        StartCoroutine(LoadLevels(_currentPage));
        
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
