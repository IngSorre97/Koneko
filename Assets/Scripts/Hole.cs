using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public int spaceLeft;
    private int _maxCapacity;
    public void Set(int capacity)
    {
        _maxCapacity = capacity;
        spaceLeft = _maxCapacity;
    }

    public bool CanEnter(bool isHeavy)
    {
        
        return spaceLeft > (isHeavy ? 1 : 0);
    }

    public void Enter(bool isHeavy)
    {
        spaceLeft = spaceLeft - (isHeavy ? 2 : 1);
    }
}
