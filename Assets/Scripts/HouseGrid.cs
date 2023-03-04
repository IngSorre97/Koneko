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
    

    public void ParseFile(StreamReader reader)
    {
        string line;
        int lineIndex = 0;
        line = reader.ReadLine();
        String[] parameters = line.Split(" ");
        rows = int.Parse(parameters[0]);
        columns = int.Parse(parameters[1]);
        while ( (line = reader.ReadLine()) != null)
        {
            gridLayout.Add(new List<GameObject>());
            for (int i = 0; i < line.Length; i++)
            {
                gridLayout[lineIndex].Add(CreateTile(line[i], lineIndex, i));
            }
            lineIndex++;
        }

        Manager.Singleton.FinishGrid();
    }

    private GameObject CreateTile(char c, int row, int column)
    {
        GameObject tile = Instantiate(tilePrefab, tilesTransform);
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
