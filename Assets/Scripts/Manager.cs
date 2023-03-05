using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Enums;
using Unity.VisualScripting;

public class Manager : MonoBehaviour
{
    public static Manager Singleton;
    public bool debug = true;
    public float rotatingDuration = 1.0f;
    public float durationBetween = 0.5f;
    
    [SerializeField] private GameObject gridPrefab;
    private GameObject grid;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject catPrefab;
    private GameObject _cat;
    private int moves;
    [SerializeField] private GameObject mousePrefab;
    private List<GameObject> _mice = new List<GameObject>();
    
    private Controller _controller;
    private GameState _state = GameState.Menu;
    private TextAsset _currentLevel;
    

    void Start()
    {
        if (Singleton == null) Singleton = this;
        else return;
        if (UIManager.Singleton == null)
            gameObject.GetComponent<UIManager>().UpdateState(_state);
        else UIManager.Singleton.UpdateState(_state);
    }

    public void SelectedLevel(TextAsset level)
    {
        _currentLevel = level;
        StartGrid(_currentLevel);
    }


    private void StartGrid(TextAsset level)
    {
        grid = Instantiate(gridPrefab);
        HouseGrid houseGrid = grid.GetComponent<HouseGrid>();
        if (houseGrid == null) return;
        houseGrid.ParseFile(level.text);
    }

    public void FinishGrid()
    {
        _state = GameState.Playing;
        UIManager.Singleton.UpdateState(_state);
        UIManager.Singleton.UpdateMouse(_mice.Count);
        UIManager.Singleton.UpdateMoves(moves);
        UIManager.Singleton.StartTimeCoroutine();
    }
    
    public void SpawnCat(GameObject houseTile)
    {
        _cat = Instantiate(catPrefab, houseTile.transform);
        _cat.name = "Cat";
        _cat.GetComponent<Cat>().Set(houseTile.GetComponent<HouseTile>());
    }

    public void SpawnMouse(GameObject houseTile)
    {
        GameObject mouse = Instantiate(mousePrefab, houseTile.transform);
        mouse.GetComponent<Mouse>().Set(houseTile.GetComponent<HouseTile>());
        mouse.name = "Mouse";
        _mice.Add(mouse);
    }

    public void Move()
    {
        moves++;
        UIManager.Singleton.UpdateMoves(moves);
    }

    public bool CanMove()
    {
        if (_state == GameState.Playing) return true;
        return false;
    }

    public void HideMouse(GameObject mouse)
    {
        _mice.Remove(mouse);
        if (debug) Debug.Log($"The mouse in {mouse.GetComponent<Mouse>()._currentTile.name} has been hided!");
        Destroy(mouse);
        UIManager.Singleton.UpdateMouse(_mice.Count);
        if (_mice.Count == 0)
            FinishGame();
    }

    private void FinishGame()
    {
        _state = GameState.Victory;
        UIManager.Singleton.UpdateState(_state);
    }

    public void OnResetClicked()
    {
        UIManager.Singleton.Reset();
        Destroy(grid);
        _mice.Clear();
        moves = 0;
        StartGrid(_currentLevel);
    }
    
}