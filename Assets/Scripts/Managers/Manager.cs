using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Enums;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager Singleton;
    public bool debug = true;
    public float rotatingDuration = 0.05f;
    public float durationBetween = 0.05f;
    public float minZoom = 5;
    public float maxZoom = 13;
    
    [SerializeField] private GameObject gridPrefab;
    private GameObject grid;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject catPrefab;
    private GameObject _cat;
    public GameObject cat => _cat;
    private int moves;
    [SerializeField] private GameObject mousePrefab;
    [SerializeField] private GameObject fastMousePrefab;
    private List<GameObject> _mice = new List<GameObject>();

    [SerializeField] private GameObject ballPrefab;

    private Controller _controller;
    private GameState _state = GameState.Menu;
    public GameState state => _state;
    public UIManager.LevelFile _currentLevel = null;

    

    void Start()
    {
        if (Singleton == null) Singleton = this;
        else return;
        if (UIManager.Singleton == null)
            gameObject.GetComponent<UIManager>().StartGame();
        else UIManager.Singleton.StartGame();
    }

    public void SelectedLevel(UIManager.LevelFile level)
    {
        ClearGame();
        UIManager.Singleton.Reset(false);
        _currentLevel = level;
        StartGrid(_currentLevel, false);
    }
    private void ClearGame()
    {
        
        Destroy(grid);
        _mice.Clear();
        
        Destroy(_cat);
        _cat = null;
        moves = 0;
    }

    public int GetMoves()
    {
        return moves;
    }

    private void StartGrid(UIManager.LevelFile level, bool reset)
    {
        grid = Instantiate(gridPrefab);
        HouseGrid houseGrid = grid.GetComponent<HouseGrid>();
        if (houseGrid == null) return;
        houseGrid.ParseFile(level.level, reset);
    }

    public void FinishGrid()
    {
        _state = GameState.Playing;
        UIManager.Singleton.UpdateState(_state);
        UIManager.Singleton.UpdateMouse(_mice.Count);
        UIManager.Singleton.UpdateMoves(moves);
        UIManager.Singleton.StartTimeCoroutine();
        StartCoroutine(FollowCat());
        _cat.GetComponent<Cat>().MakeParticles();
    }
    
    public void SpawnCat(GameObject houseTile)
    {
        _cat = Instantiate(catPrefab, houseTile.transform);
        _cat.name = "Cat";
        _cat.GetComponent<Cat>().Set(houseTile.GetComponent<HouseTile>());
        MovementManager.Singleton.cat = _cat.GetComponent<Cat>();
    }

    private IEnumerator FollowCat()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        while (true)
        {
            if (_cat == null)
                break;
            cameraPosition.x = _cat.transform.position.x;
            cameraPosition.y = _cat.transform.position.y;
            mainCamera.transform.position = cameraPosition;
            yield return null;
        }
        yield return null;
    }

    public void SpawnMouse(GameObject houseTile, bool isFast)
    {
        GameObject mouse = Instantiate(isFast ? fastMousePrefab : mousePrefab, houseTile.transform);
        mouse.GetComponent<Mouse>().Set(houseTile.GetComponent<HouseTile>());
        mouse.GetComponent<Mouse>().isSlippery = isFast;
        mouse.name = "Mouse";
        _mice.Add(mouse);
    }

    public void SpawnBall(GameObject houseTile)
    {
        GameObject ball = Instantiate(ballPrefab, houseTile.transform);
        ball.GetComponent<Ball>().Set(houseTile.GetComponent<HouseTile>());
        ball.name = "Ball";
    }

    public void Move()
    {
        moves++;
        UIManager.Singleton.UpdateMoves(moves);
    }

    public bool CanMove()
    {
        if (_state == GameState.Playing && !MovementManager.Singleton.IsSomethingMoving()) return true;
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
        var record = new RecordManager.RecordData();

        record.nickname = UIManager.Singleton.nickname;
        record.moves = moves;
        string timeString = UIManager.Singleton.GetTime().TrimEnd("s");
        
        record.minutes = int.Parse(timeString.Split(":")[0].TrimEnd("m"));
        record.seconds = int.Parse(timeString.Split(":")[1]);

        RecordManager.Singleton.AddRecord(UIManager.Singleton._currentID.ToString() ,record);

        UIManager.Singleton.UpdateState(_state);
        
    }


    public void OnResetClicked()
    {
        if (_currentLevel == null) return;
        UIManager.Singleton.Reset(false);
        Destroy(grid);
        _mice.Clear();
        moves = 0;
        StartGrid(_currentLevel, true);
    }
    public void OnBackMenu()
    {
        ClearGame();
        UIManager.Singleton.Reset(true);
        _state = GameState.Menu;
        UIManager.Singleton.UpdateState(_state);
    }

    public void OnZoomInClicked()
    {
        SoundManager.Instance.Button();
        if (_currentLevel == null) return;
        float size = mainCamera.GetComponent<Camera>().orthographicSize;
        mainCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(minZoom, size - 1);
    }

    public void OnZoomOutClicked()
    {
        SoundManager.Instance.Button();
        if (_currentLevel == null) return;
        float size = mainCamera.GetComponent<Camera>().orthographicSize;
        mainCamera.GetComponent<Camera>().orthographicSize = Mathf.Min(maxZoom, size + 1);
    }
    
}