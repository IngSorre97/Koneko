using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public HouseTile _currentTile { get; private set; }

    public void Set(HouseTile houseTile)
    {
        _currentTile = houseTile;
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
}
