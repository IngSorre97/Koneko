using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : PushableEntity
{
    public HouseTile _currentTile { get; private set; }
    private Movement _currentMovement;
    [SerializeField] private ParticleSystem holeDust;

    public void Set(HouseTile houseTile)
    {
        _currentTile = houseTile;
    }
    public override void Move(GameObject houseTile, Movement movement)
    {
        _currentMovement = movement;
        gameObject.GetComponent<Animator>().SetInteger("Movement", (int)_currentMovement);
        _currentTile = houseTile.GetComponent<HouseTile>();
        StartCoroutine(MoveRoutine(0.25f, transform.position, houseTile.transform.position));
    }

    private IEnumerator MoveRoutine(float duration, Vector3 start, Vector3 end)
    {
        MovementManager.Singleton.MouseMovement(true, this);
        float time = 0;
        float fraction = Mathf.Min(time / duration, 1.0f);
        while (time < duration)
        {
            time += Time.deltaTime;
            fraction = Mathf.Min(time / duration, 1);
            transform.position = Vector3.Lerp(start, end, fraction);
            yield return null;
        }
        if (_currentTile.tileType == TileType.Hole)
        {
            StartCoroutine(particles());
            _currentTile.EnterMouse();
        }
        else
        {
            _currentTile.tileType = TileType.Mouse;
            transform.SetParent(_currentTile.gameObject.transform);
            _currentMovement = Movement.None;
        }

        MovementManager.Singleton.MouseMovement(false, this);

        yield return null;
    }

    private IEnumerator particles()
    {
        holeDust.Play();
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        Manager.Singleton.HideMouse(gameObject);
    }
}
