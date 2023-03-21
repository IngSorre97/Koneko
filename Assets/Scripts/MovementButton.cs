using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] public Movement movement;

    public bool isMoving = false;
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (!isMoving) return;
        MovementManager.Singleton.AskMovement(Movement.None, this);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        MovementManager.Singleton.AskMovement(movement, this);
    }

}
