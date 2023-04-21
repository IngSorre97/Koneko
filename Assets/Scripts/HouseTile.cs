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
                Manager.Singleton.SpawnMouse(gameObject, MouseType.Basic);
                grid.mouseTiles.Add(gameObject);
                break;
            case 'F':
                tileType = TileType.Mouse;
                sprite.sprite = SpritesData.Singleton.empty;
                Manager.Singleton.SpawnMouse(gameObject, MouseType.Fast);
                grid.mouseTiles.Add(gameObject);
                break;
            case 'H':
                tileType = TileType.Mouse;
                sprite.sprite = SpritesData.Singleton.empty;
                Manager.Singleton.SpawnMouse(gameObject, MouseType.Heavy);
                grid.mouseTiles.Add(gameObject);
                break;
            case char ch when (ch >= '1'  && ch <='9'):
                tileType = TileType.Hole;
                hole = gameObject.AddComponent<Hole>();
                hole.Set(ch - '0');
                sprite.sprite = SpritesData.Singleton.Hole(ch - '0');
                break;
            case 'B':
                tileType = TileType.Ball;
                sprite.sprite = SpritesData.Singleton.empty;
                Manager.Singleton.SpawnBall(gameObject);
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
        return tileType == TileType.Empty || tileType == TileType.Mouse || tileType == TileType.Ball;
    }
    
    public bool CanMoveEntity(bool isHeavy)
    {
        if (tileType == TileType.Empty) return true;
        if (tileType == TileType.Hole) return hole.CanEnter(isHeavy);
        return false;
    }

    public void EnterHole(bool isHeavy)
    {
        hole.Enter(isHeavy);
        sprite.sprite = SpritesData.Singleton.Hole(hole.spaceLeft);
    }
}
