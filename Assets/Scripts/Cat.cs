using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : PushableEntity
{
    private Controller _controller;
    public HouseTile currentTile;
    [SerializeField] private ParticleSystem resetDust;
    private Movement _currentMovement;

    public void Set(HouseTile houseTile)
    {
        currentTile = houseTile;
        
        _controller = new Controller();
        _controller.Enable();

        
        _controller.Gameplay.Left.performed += ctx =>
        {       
            MovementManager.Singleton.TryMovement(Movement.Left);
        };
        _controller.Gameplay.Right.performed += ctx =>
        {
            MovementManager.Singleton.TryMovement(Movement.Right);
        };
        _controller.Gameplay.Up.performed += ctx =>
        {
            MovementManager.Singleton.TryMovement(Movement.Up);
        };
        _controller.Gameplay.Down.performed += ctx =>
        {
            MovementManager.Singleton.TryMovement(Movement.Down);
        };
        
        _controller.Gameplay.Menu.performed += ctx =>
        {
            Application.Quit();
        };
    }

    public override void Move(GameObject houseTile, Movement movement)
    {
        currentTile = houseTile.GetComponent<HouseTile>();
        transform.SetParent(houseTile.transform, true);
        _currentMovement = movement;
        gameObject.GetComponent<Animator>().SetInteger("Movement", (int) movement);
        StartCoroutine(MoveRoutine(0.25f, transform.position, houseTile.transform.position));
    }

    private IEnumerator MoveRoutine(float duration, Vector3 start, Vector3 end)
    {
        MovementManager.Singleton.CatMovement(true);
        float time = 0;
        float fraction = Mathf.Min(time / duration, 1.0f);
        while (time < duration)
        {
            time += Time.deltaTime;
            fraction = Mathf.Min(time / duration, 1);
            transform.position = Vector3.Lerp(start, end, fraction);
            yield return null;
        }
        currentTile.tileType = TileType.Mouse;
        transform.SetParent(currentTile.gameObject.transform);
        _currentMovement = Movement.None;
        
        MovementManager.Singleton.CatMovement(false);
        yield return null;
    }

    private void OnDestroy()
    {
        _controller.Disable();
    }

    public void MakeParticles()
    {
        resetDust.Play();
    }
}
