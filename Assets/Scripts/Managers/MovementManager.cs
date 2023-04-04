using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Singleton;
    public float mouseDuration = 0.25f;
    public float catDuration = 0.25f;
    [HideInInspector] public HouseGrid grid;
    [HideInInspector] public Cat cat;
    private Movement storedMovement = Movement.None;
    private Movement currentMovement = Movement.None;
    private int entitiesMoving = 0;

    public static Vector2 GetMovementCoords(Movement movement)
    {
        switch(movement)
        {
            case Movement.Up:
                return Vector2.up;
            case Movement.Right:
                return Vector2.right;
            case Movement.Down:
                return Vector2.down;
            case Movement.Left:
                return Vector2.left;

            default:
                return Vector2.zero;
        }
    }
    private void Start()
    {
        if (Singleton == null) Singleton = this;
        else return;
    }

    public void AskMovement(Movement askedMovement, MovementButton button)
    {
        if (Manager.Singleton.state != Enums.GameState.Playing) return;

        if (storedMovement == Movement.None)
        {
            storedMovement = askedMovement;
            button.isMoving = true;
            if (currentMovement == Movement.None)
            {
                bool result = TryMovement(storedMovement, false);
                currentMovement = result ? storedMovement : Movement.None;
            }
        } else
        {
            if (askedMovement != Movement.None) return;
            button.isMoving = false;
            storedMovement = Movement.None;
        }
    }

    public bool TryMovement(Movement movement, bool redo)
    {
        if (!Manager.Singleton.CanMove()) return false;

        HouseTile targetTile = grid.TryMovementBounds(cat.currentTile, movement);
        if (!targetTile) return false;
        if (!targetTile.IsWalkable()) return false;
        switch (targetTile.tileType)
        {
            case TileType.Empty:
                targetTile.tileType = TileType.Cat;
                cat.currentTile.tileType = TileType.Empty;
                cat.Move(targetTile.gameObject, movement, redo, 1);
                currentMovement = movement;
                Manager.Singleton.Move();
                return true;
            case TileType.Mouse:
                GameObject mouse = targetTile.transform.Find("Mouse").gameObject;
                HouseTile mouseTargetTile = grid.TryMovementBounds(targetTile, movement);
                if (!mouseTargetTile) return false;
                if (!mouseTargetTile.CanMoveEntity()) return false;
                if (mouse.GetComponent<Mouse>().isSlippery)
                {
                    HouseGrid.TargetData targetData = grid.GetSlipperyTarget(mouseTargetTile, movement);
                    mouse.GetComponent<Mouse>().Move(targetData.houseTiletargetTile.gameObject, movement, redo, targetData.distance);
                } else
                    mouse.GetComponent<Mouse>().Move(mouseTargetTile.gameObject, movement, redo, 1);
                
                targetTile.tileType = TileType.Cat;
                cat.currentTile.tileType = TileType.Empty;
                cat.Move(targetTile.gameObject, movement, redo, 1);
                if (!redo)
                    SoundManager.Instance.Push();
                currentMovement = movement;
                Manager.Singleton.Move();
                return true;
            case TileType.Ball:
                GameObject ball = targetTile.transform.Find("Ball").gameObject;
                HouseTile ballTargetTile = grid.TryMovementBounds(targetTile, movement);
                if (!ballTargetTile) return false;
                if (!ballTargetTile.CanMoveEntity()) return false;
                ball.GetComponent<Ball>().Move(ballTargetTile.gameObject, movement, redo, 1);
                targetTile.tileType = TileType.Cat;
                cat.currentTile.tileType = TileType.Empty;
                cat.Move(targetTile.gameObject, movement, redo, 1);
                if (!redo)
                    SoundManager.Instance.Push();
                currentMovement = movement;
                Manager.Singleton.Move();
                return true;
        }

        return false;
    }

    public void CatMovement(bool starting)
    {
        entitiesMoving = starting ? entitiesMoving + 1 : entitiesMoving - 1;
        if (!starting)
        {
            if (entitiesMoving == 0)
                if (RedoMovement()) return;
            cat.gameObject.GetComponent<Animator>().SetFloat("MovementX", 0);
            cat.gameObject.GetComponent<Animator>().SetFloat("MovementY", 0);
        }
    }

    public void MouseMovement(bool starting, Mouse mouse)
    {
        entitiesMoving = starting ? entitiesMoving + 1 : entitiesMoving - 1;
        if (!starting)
        {
            if (entitiesMoving == 0)
                if (RedoMovement()) return;
            mouse.gameObject.GetComponent<Animator>().SetFloat("MovementX", 0);
            mouse.gameObject.GetComponent<Animator>().SetFloat("MovementY", 0);
        }
    }

    public void BallMovement(bool starting, Ball ball)
    {
        entitiesMoving = starting ? entitiesMoving + 1 : entitiesMoving - 1;

        if (!starting)
        {
            if (entitiesMoving == 0)
                if (RedoMovement()) return;
            ball.gameObject.GetComponent<Animator>().SetFloat("MovementX", 0);
            ball.gameObject.GetComponent<Animator>().SetFloat("MovementY", 0);
        }
    }

    private bool RedoMovement()
    {
        currentMovement = Movement.None;
        if (storedMovement != Movement.None)
            return TryMovement(storedMovement, true);
        return false;
    }

    public bool IsSomethingMoving()
    {
        return entitiesMoving != 0;
    }


}
