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

    private Hole hole;

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
                sprite.sprite = SpritesData.Singleton.outOfBounds;
                break;
            case 'W':
                tileType = TileType.Wall;
                sprite.sprite = SpritesData.Singleton.wall;
                break;
            case 'E':
                tileType = TileType.Empty;
                sprite.sprite = SpritesData.Singleton.empty;
                break;
            case 'C':
                tileType = TileType.Cat;
                sprite.sprite = SpritesData.Singleton.empty;
                Manager.Singleton.SpawnCat(gameObject);
                grid.spawnTile = gameObject;
                break;
            case 'M':
                tileType = TileType.Mouse;
                sprite.sprite = SpritesData.Singleton.empty;
                Manager.Singleton.SpawnMouse(gameObject);
                grid.mouseTiles.Add(gameObject);
                break;
            case char ch when (ch >= '1'  && ch <='9'):
                tileType = TileType.Hole;
                hole = new Hole(ch - '0');
                sprite.sprite = SpritesData.Singleton.Hole(ch - '0');
                break;
            default:
                Debug.Log($"Character not found {c}");
                break;
                
        }
    }

    public IEnumerator Show(float duration)
    {
        yield return new WaitForSeconds(Random.Range(0f, 1f) * 0.5f);
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
    
    public bool CanMoveMouse()
    {
        if (tileType == TileType.Empty) return true;
        if (tileType == TileType.Hole) return hole.CanEnter();
        return false;
    }

    public void EnterMouse()
    {
        hole.Enter();
        sprite.sprite = SpritesData.Singleton.Hole(hole.spaceLeft);
    }
    
}
