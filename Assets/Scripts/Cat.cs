using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    private Controller _controller;
    private HouseTile _currentTile;
    
    public void Set(HouseTile houseTile)
    {
        _currentTile = houseTile;
        
        _controller = new Controller();
        _controller.Enable();

        _controller.Gameplay.Left.performed += ctx =>
        {
            Movement(global::Movement.Left);
        };
        _controller.Gameplay.Right.performed += ctx =>
        {
            Movement(global::Movement.Right);
        };
        _controller.Gameplay.Up.performed += ctx =>
        {
            Movement(global::Movement.Up);
        };
        _controller.Gameplay.Down.performed += ctx =>
        {
            Movement(global::Movement.Down);
        };
        _controller.Gameplay.Menu.performed += ctx =>
        {
            Application.Quit();
        };
    }

    private void Movement(Movement movement)
    {
        if (!Manager.Singleton.CanMove()) return;
        if (_currentTile.TryMovement(movement, this))
            Manager.Singleton.Move();
    }

    public void Move(GameObject houseTile)
    {
        _currentTile = houseTile.GetComponent<HouseTile>();
        transform.SetParent(houseTile.transform, true);
        StartCoroutine(MoveRoutine(0.5f, transform.position, houseTile.transform.position));
    }

    private IEnumerator MoveRoutine(float duration, Vector3 start, Vector3 end)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, time / duration);
        }
        yield return null;
    }

    private void OnDestroy()
    {
        _controller.Disable();
    }
}
