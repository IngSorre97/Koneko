using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    private Controller _controller;
    private HouseTile _currentTile;
    private Movement _currentMovement;
    
    public void Set(HouseTile houseTile)
    {
        _currentTile = houseTile;
        
        _controller = new Controller();
        _controller.Enable();

        _controller.Gameplay.Left.performed += ctx =>
        {
            TryMovement(Movement.Left);
        };
        _controller.Gameplay.Right.performed += ctx =>
        {
            TryMovement(Movement.Right);
        };
        _controller.Gameplay.Up.performed += ctx =>
        {
            TryMovement(Movement.Up);
        };
        _controller.Gameplay.Down.performed += ctx =>
        {
            TryMovement(Movement.Down);
        };
        _controller.Gameplay.Menu.performed += ctx =>
        {
            Application.Quit();
        };
    }

    private void TryMovement(Movement movement)
    {
        if (!Manager.Singleton.CanMove()) return;
        if (_currentTile.TryMovement(movement, this))
            Manager.Singleton.Move();
    }

    public void Move(GameObject houseTile, Movement movement)
    {
        _currentTile = houseTile.GetComponent<HouseTile>();
        transform.SetParent(houseTile.transform, true);
        _currentMovement = movement;
        StartCoroutine(MoveRoutine(0.25f, transform.position, houseTile.transform.position));
    }

    private IEnumerator MoveRoutine(float duration, Vector3 start, Vector3 end)
    {
        Manager.Singleton.catMoving = true;
        gameObject.GetComponent<Animator>().SetInteger("Movement", (int)_currentMovement);
        float time = 0;
        float fraction = Mathf.Min(time / duration, 1.0f);
        while (time < duration)
        {
            time += Time.deltaTime;
            fraction = Mathf.Min(time / duration, 1);
            transform.position = Vector3.Lerp(start, end, fraction);
            yield return null;
        }
        _currentTile.tileType = TileType.Mouse;
        transform.SetParent(_currentTile.gameObject.transform);
        _currentMovement = Movement.None;
        gameObject.GetComponent<Animator>().SetInteger("Movement", (int)_currentMovement);

        Manager.Singleton.catMoving = false;
        _currentMovement = Movement.None;
        gameObject.GetComponent<Animator>().SetInteger("Movement", (int)_currentMovement);
        
        yield return null;
    }

    private void OnDestroy()
    {
        _controller.Disable();
    }
}
