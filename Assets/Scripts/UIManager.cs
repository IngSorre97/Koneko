using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Enums;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
                break;
        }
    }

    private IEnumerator FadingMenu(float duration)
    {
        CanvasGroup canvasGroup = menuScreen.GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(2.0f);
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
        foreach (Object level in Resources.LoadAll("Levels/"))
        {
            
            GameObject placeholder = Instantiate(placeholderPrefab, levelsContent);
            placeholder.GetComponent<Placeholder>().level = (TextAsset) level;
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
    }
}
