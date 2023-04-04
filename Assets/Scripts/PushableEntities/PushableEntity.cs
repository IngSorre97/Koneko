using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PushableEntity : MonoBehaviour
{
    public abstract void Move(GameObject startTile, Movement movement, bool redo, int distance);
}
