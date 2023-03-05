using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class HouseGrid : MonoBehaviour
{
    private int rows;
    private int columns;
    [SerializeField] private Transform tilesTransform;
    [SerializeField] private GameObject tilePrefab;
    
    
    private List<List<GameObject>> gridLayout = new List<List<GameObject>>();
    public GameObject spawnTile;
    public List<GameObject> mouseTiles = new List<GameObject>();
    

    public void ParseFile(string level, bool reset)
    {
        string[] lines = level.Split("\n");
        String[] parameters = lines[0].Split(" ");
        rows = int.Parse(parameters[0]);
        columns = int.Parse(parameters[1]);
        for (int i=1; i<rows+1; i++)
        {
            string line = lines[i];
            gridLayout.Add(new List<GameObject>());
            for (int j = 0; j < columns; j++)
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
        
        for (int i=0;i < rows;i++)
        {
            for (int j = 0; j < columns; j++)
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
                if (houseTile.column == columns - 1) return null;
                return gridLayout[houseTile.row][houseTile.column + 1].GetComponent<HouseTile>();

            case Movement.Left:
                if (houseTile.column == 0) return null;
                return gridLayout[houseTile.row][houseTile.column - 1].GetComponent<HouseTile>();
            
            case Movement.Up:
                if (houseTile.row == rows - 1) return null;
                return gridLayout[houseTile.row + 1][houseTile.column].GetComponent<HouseTile>();
            
            case Movement.Down:
                if (houseTile.row == 0) return null;
                return gridLayout[houseTile.row - 1][houseTile.column].GetComponent<HouseTile>();
            
            default:
                return null;
        }
    }
}
