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
    [SerializeField] private GameObject gridPrefab;
    private GameObject grid;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject catPrefab;
    private GameObject _cat;
    private int moves;
    [SerializeField] private GameObject mousePrefab;
    private List<GameObject> _mice = new List<GameObject>();
    
    private string path = "Assets/Resources/test.txt";
    private Controller _controller;
    private GameState _state = GameState.Menu;
    

    void Start()
    {
        if (Singleton == null) Singleton = this;
        else return;
        
        StartCoroutine(WaitScreen());

    }

    private IEnumerator WaitScreen()
    {
        yield return new WaitForSeconds(3.0f);
        _state = GameState.Playing;
        UIManager.Singleton.UpdateState(_state);
        StartGrid();
        yield return null;
    }


    private void StartGrid()
    {
        grid = Instantiate(gridPrefab);
        HouseGrid houseGrid = grid.GetComponent<HouseGrid>();
        if (houseGrid == null) return;
        StreamReader reader = new StreamReader(path); 
        houseGrid.ParseFile(reader);
    }

    public void FinishGrid()
    {
        UIManager.Singleton.UpdateMouse(_mice.Count);
        UIManager.Singleton.UpdateMoves(moves);
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
        StartGrid();

    }
    
}