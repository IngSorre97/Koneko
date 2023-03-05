using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HouseTile : MonoBehaviour
{
    public TileType tileType;
    public HouseGrid grid;

    public int row { get; private set; }
    public int column { get; private set; }
    public bool showing;

    [SerializeField] private GameObject transparency;
    
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer tileBackground;

    public void SetTile(HouseGrid houseGrid, char c, int row, int column)
    {
        grid = houseGrid;
        this.row = row;
        this.column = column;

        SetSprite(c);
    }

    private void SetSprite(char c)
    {
        switch (c)
        {
            case 'O':
                tileType = TileType.OutOfBounds;
                transparency.SetActive(false);
                break;
            case 'W':
                tileType = TileType.Wall;
                tileBackground.color = Color.black;
                sprite.sprite = null;
                break;
            case 'E':
                tileType = TileType.Empty;
                sprite.sprite = null;
                break;
            case 'C':
                tileType = TileType.Cat;
                sprite.sprite = null;
                Manager.Singleton.SpawnCat(gameObject);
                grid.spawnTile = gameObject;
                break;
            case 'M':
                tileType = TileType.Mouse;
                sprite.sprite = null;
                Manager.Singleton.SpawnMouse(gameObject);
                grid.mouseTiles.Add(gameObject);
                break;
            case 'H':
                tileType = TileType.Hole;
                sprite.color = Color.red;
                break;
            default:
                break;
        }
    }

    public IEnumerator Show(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float fraction = Mathf.Min(1, time / duration);
            float y = Mathf.LerpAngle(90, 0, fraction);
            transform.eulerAngles = new Vector3(y, 0, 0);
            time += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    public bool IsWalkable()
    {
        return tileType == TileType.Empty || tileType == TileType.Mouse;
    }
    
    public bool TryMovement(Movement movement, Cat cat)
    {
        HouseTile targetTile = grid.TryMovementBounds(this, movement);
        if (!targetTile) return false;
        if (!targetTile.IsWalkable()) return false;
        switch (targetTile.tileType)
        {
            case TileType.Empty:
                targetTile.tileType = TileType.Cat;
                cat.Move(targetTile.gameObject);
                tileType = TileType.Empty;
                return true;
            case TileType.Mouse:
                GameObject mouse = targetTile.transform.Find("Mouse").gameObject;
                HouseTile mouseTargetTile = grid.TryMovementBounds(targetTile, movement);
                if (!mouseTargetTile) return false;
                if (!mouseTargetTile.CanMoveMouse()) return false;
                if (mouseTargetTile.tileType == TileType.Hole)
                {
                    Manager.Singleton.HideMouse(mouse);
                }
                else
                {
                    mouseTargetTile.tileType = TileType.Mouse;
                    mouse.GetComponent<Mouse>().Move(mouseTargetTile.gameObject);
                }
                targetTile.tileType = TileType.Cat;
                cat.Move(targetTile.gameObject);
                tileType = TileType.Empty;
                return true;
        }
        
        return false;
    }

    public bool CanMoveMouse()
    {
        return tileType == TileType.Empty || tileType == TileType.Hole;
    }
    
}
