using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HoldButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool hovered;

    public UnityEvent whileClicked;

    void Update() 
    {
        if (hovered && Mouse.current.leftButton.ReadValue() > 0)
        {
            whileClicked.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}
