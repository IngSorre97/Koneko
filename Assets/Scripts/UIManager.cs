using System.Collections;
using System.Collections.Generic;
using Enums;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Singleton;

    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject counters;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private TextMeshProUGUI miceLeft;
    [SerializeField] private TextMeshProUGUI movesCounter;
    
    void Start()
    {
        if (Singleton == null) Singleton = this;
        else return;
        menuScreen.SetActive(true);
    }

    public void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                menuScreen.SetActive(false);
                counters.SetActive(true);
                break;
            case GameState.Victory:
                counters.SetActive(false);
                victoryScreen.SetActive(true);
                break;
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
