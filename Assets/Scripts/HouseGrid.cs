using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class HouseGrid : MonoBehaviour
{
    private int _rows;
    private int _columns;
    private int _bestMoves;
    private int _goodMoves;
    
    [SerializeField] private Transform tilesTransform;
    [SerializeField] private GameObject tilePrefab;
    
    
    private List<List<GameObject>> gridLayout = new List<List<GameObject>>();
    public GameObject spawnTile;
    public List<GameObject> mouseTiles = new List<GameObject>();

    private void Start()
    {
        MovementManager.Singleton.grid = this;
    }
    public void ParseFile(TextAsset textAsset, bool reset)
    {
        string level = textAsset.text;
        string[] lines = level.Split("\n");
        String[] parameters = lines[0].Split(" ");
        _rows = int.Parse(parameters[0]);
        _columns = int.Parse(parameters[1]);
        _bestMoves = int.Parse(parameters[2]);
        _goodMoves = int.Parse(parameters[3]);
        
        for (int i=1; i<_rows+1; i++)
        {
            string line = lines[i];
            gridLayout.Add(new List<GameObject>());
            for (int j = 0; j < _columns; j++)
            {
                gridLayout[i-1].Add(CreateTile(line[j], i-1, j, reset));
            }
        }

        if (!reset)
            StartCoroutine(ShowGrid());
        else
            Manager.Singleton.FinishGrid();
    }

    private IEnumerator ShowGrid()
    {
        List<GameObject> toBeShown = new List<GameObject>{spawnTile};
        spawnTile.GetComponent<HouseTile>().showing = true;
        
        for (int i=0;i < _rows;i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                StartCoroutine(gridLayout[i][j].GetComponent<HouseTile>().Show(Manager.Singleton.rotatingDuration));
            }
        }

        yield return new WaitForSeconds(Manager.Singleton.rotatingDuration + 0.5f);
        Manager.Singleton.FinishGrid();
        yield return null;
    }

    
    private GameObject CreateTile(char c, int row, int column, bool reset)
    {
        GameObject tile = Instantiate(tilePrefab, tilesTransform);
        tile.transform.Rotate(0, reset ? 0 : 90, 0);
        tile.transform.localPosition = new Vector3(column, -row, 0);
        tile.name = row + "-" + column;
        tile.GetComponent<HouseTile>().SetTile(this, c, row, column);
        
        return tile;
    }
    
    public HouseTile TryMovementBounds(HouseTile houseTile, Movement movement)
    {
        switch (movement)
        {
            case Movement.Right:
                if (houseTile.column == _columns - 1) return null;
                return gridLayout[houseTile.row][houseTile.column + 1].GetComponent<HouseTile>();

            case Movement.Left:
                if (houseTile.column == 0) return null;
                return gridLayout[houseTile.row][houseTile.column - 1].GetComponent<HouseTile>();
            
            case Movement.Up:
                if (houseTile.row == _rows - 1) return null;
                return gridLayout[houseTile.row - 1][houseTile.column].GetComponent<HouseTile>();
            
            case Movement.Down:
                if (houseTile.row == 0) return null;
                return gridLayout[houseTile.row + 1][houseTile.column].GetComponent<HouseTile>();
            
            default:
                return null;
        }
    }

    public struct TargetData
    {
        public HouseTile houseTiletargetTile;
        public int distance;
    }
    public TargetData GetSlipperyTarget(HouseTile houseTile, Movement movement, bool isHeavy)
    {
        int distance = 0;
        HouseTile nextTile = houseTile;
        do
        {
            houseTile = nextTile;
            distance++;
            nextTile = TryMovementBounds(houseTile, movement);
            if (nextTile.tileType == TileType.Hole && nextTile.CanMoveEntity(isHeavy))
            {
                houseTile = nextTile;
                break;
            }
            if (!nextTile.CanMoveEntity(isHeavy))
                nextTile = null;
            
        } while (nextTile != null);

        return new TargetData { houseTiletargetTile = houseTile, distance = distance};
    }
}
