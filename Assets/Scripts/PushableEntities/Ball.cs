using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : PushableEntity
{
    public HouseTile _currentTile { get; private set; }
    private Movement _currentMovement;
    [SerializeField] private AnimationCurve movementCurve;
    [SerializeField] private ParticleSystem holeDust;

    public void Set(HouseTile houseTile)
    {
        _currentTile = houseTile;
    }
    public override void Move(GameObject houseTile, Movement movement, bool redo, int distance)
    {
        _currentMovement = movement;
        Vector2 animMovement = MovementManager.GetMovementCoords(movement);
        gameObject.GetComponent<Animator>().SetFloat("MovementX", animMovement.x);
        gameObject.GetComponent<Animator>().SetFloat("MovementY", animMovement.y);
        _currentTile = houseTile.GetComponent<HouseTile>();
        StartCoroutine(MoveRoutine(MovementManager.Singleton.mouseDuration, transform.position, houseTile.transform.position, redo));
    }
    private IEnumerator MoveRoutine(float duration, Vector3 start, Vector3 end, bool redo)
    {
        MovementManager.Singleton.BallMovement(true, this);
        float time = 0;
        float fraction = Mathf.Min(time / duration, 1.0f);
        while (time < duration)
        {
            time += Time.deltaTime;
            fraction = Mathf.Min(time / duration, 1);
            transform.position = Vector3.Lerp(start, end, redo ? fraction : movementCurve.Evaluate(fraction));
            yield return null;
        }
        if (_currentTile.tileType == TileType.Hole)
        {
            StartCoroutine(particles());
            _currentTile.EnterHole();
        }
        else
        {
            _currentTile.tileType = TileType.Ball;
            transform.SetParent(_currentTile.gameObject.transform);
            _currentMovement = Movement.None;
        }

        MovementManager.Singleton.BallMovement(false, this);

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
