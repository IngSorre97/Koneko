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
    

    public void ParseFile(string level)
    {
        string[] lines = level.Split("\n");
        String[] parameters = lines[0].Split(" ");
        rows = int.Parse(parameters[0]);
        columns = int.Parse(parameters[1]);
        for (int i=1; i<lines.Length; i++)
        {
            string line = lines[i];
            gridLayout.Add(new List<GameObject>());
            for (int j = 0; j < line.Length; j++)
            {
                gridLayout[i-1].Add(CreateTile(line[j], i-1, j));
            }
        }

        StartCoroutine(ShowGrid());
    }

    private IEnumerator ShowGrid()
    {
        List<GameObject> toBeShown = new List<GameObject>{spawnTile};
        spawnTile.GetComponent<HouseTile>().showing = true;
        while (toBeShown.Count > 0)
        {
            List<GameObject> newTiles = new List<GameObject>();
            foreach (GameObject showTile in toBeShown)
            {
                StartCoroutine(showTile.GetComponent<HouseTile>().Show(Manager.Singleton.rotatingDuration));
                showTile.GetComponent<HouseTile>().showing = true;
                newTiles.AddRange(GetAdjacentTiles(showTile));
            }
            toBeShown.Clear();
            toBeShown.AddRange(newTiles);
            yield return new WaitForSeconds(Manager.Singleton.durationBetween);
        }
        
        Manager.Singleton.FinishGrid();
        yield return null;
    }

    private List<GameObject> GetAdjacentTiles(GameObject tileObject)
    {
        List<GameObject> toBeShown = new List<GameObject>();
        HouseTile startingTile = tileObject.GetComponent<HouseTile>();
        HouseTile tile;
        tile = TryMovementBounds(startingTile, Movement.Right);
        if (tile != null && !tile.showing) toBeShown.Add(tile.gameObject);
        tile = TryMovementBounds(startingTile, Movement.Left);
        if (tile != null && !tile.showing) toBeShown.Add(tile.gameObject);
        tile = TryMovementBounds(startingTile, Movement.Up);
        if (tile != null && !tile.showing) toBeShown.Add(tile.gameObject);
        tile = TryMovementBounds(startingTile, Movement.Down);
        if (tile != null && !tile.showing) toBeShown.Add(tile.gameObject);

        return toBeShown;
    }
    
    private GameObject CreateTile(char c, int row, int column)
    {
        GameObject tile = Instantiate(tilePrefab, tilesTransform);
        tile.transform.Rotate(0, 90, 0);
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
