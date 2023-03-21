using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Singleton;
    [HideInInspector] public HouseGrid grid;
    [HideInInspector] public Cat cat;
    private Movement storedMovement = Movement.None;
    private Movement currentMovement = Movement.None;
    private bool catMoving;
    private bool mouseMoving;

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
                bool result = TryMovement(storedMovement);
                currentMovement = result ? storedMovement : Movement.None;
            }
        } else
        {
            if (askedMovement != Movement.None) return;
            button.isMoving = false;
            storedMovement = Movement.None;
        }
    }

    public bool TryMovement(Movement movement)
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
                cat.Move(targetTile.gameObject, movement);
                currentMovement = movement;
                Manager.Singleton.Move();
                return true;
            case TileType.Mouse:
                GameObject mouse = targetTile.transform.Find("Mouse").gameObject;
                HouseTile mouseTargetTile = grid.TryMovementBounds(targetTile, movement);
                if (!mouseTargetTile) return false;
                if (!mouseTargetTile.CanMoveMouse()) return false;
                mouse.GetComponent<Mouse>().Move(mouseTargetTile.gameObject, movement);
                targetTile.tileType = TileType.Cat;
                cat.currentTile.tileType = TileType.Empty;
                cat.Move(targetTile.gameObject, movement);
                UIManager.Singleton.Push();
                currentMovement = movement;
                Manager.Singleton.Move();
                return true;
        }

        return false;
    }

    public void CatMovement(bool starting)
    {
        catMoving = starting;
        if (!starting)
        {
            if (!mouseMoving)
                if (RedoMovement()) return;
            cat.gameObject.GetComponent<Animator>().SetInteger("Movement", 0);
        }
    }

    public void MouseMovement(bool starting, Mouse mouse)
    {
        mouseMoving = starting;
        if (!starting)
        {
            if (!catMoving) 
                if (RedoMovement()) return;
            mouse.gameObject.GetComponent<Animator>().SetInteger("Movement", 0);
        }
        
    }

    private bool RedoMovement()
    {
        currentMovement = Movement.None;
        if (storedMovement != Movement.None)
            return TryMovement(storedMovement);
        return false;
    }

    public bool IsSomethingMoving()
    {
        return mouseMoving || catMoving;
    }


}
