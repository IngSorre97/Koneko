using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public int spaceLeft;
    private int _maxCapacity;
    public Hole(int capacity)
    {
        _maxCapacity = capacity;
        spaceLeft = _maxCapacity;
    }

    public bool CanEnter()
    {
        return spaceLeft > 0;
    }

    public void Enter()
    {
        spaceLeft--;
    }
}
