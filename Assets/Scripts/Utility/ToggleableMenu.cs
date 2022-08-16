using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleableMenu : MonoBehaviour
{
    [Header("Button")]
    public Transform buttonImage;
    public Vector3 startEuler;
    public Vector3 endEuler;
    [Header("Menu")]
    public Transform menuHolder;
    private RectTransform menuRect;
    public Vector3 menuStartPos;
    public Vector3 menuEndPos;
    [Header("Options")]
    public float moveSpeed = 1;
    public bool asLerp = true;
    [Space]
    public bool isOpen;

    private void Start()
    {
        menuHolder.TryGetComponent(out menuRect);

        if(menuRect != null) menuRect.anchoredPosition = menuStartPos;
        else menuHolder.localPosition = menuStartPos;

        buttonImage.localRotation = Quaternion.Euler(startEuler);
    }

    private void Update()
    {
        Quaternion targetRot = Quaternion.Euler(isOpen ? endEuler : startEuler);
        buttonImage.localRotation = Quaternion.Lerp(buttonImage.localRotation, targetRot, moveSpeed * Time.deltaTime);

        Vector3 targetPos = isOpen ? menuEndPos : menuStartPos;

        if(menuRect != null) 
        {
            if (asLerp)
            { 
                menuRect.anchoredPosition = Vector3.Lerp(menuRect.anchoredPosition, targetPos, moveSpeed * Time.deltaTime);
            }
            else
            {
                menuRect.anchoredPosition = Vector3.MoveTowards(menuRect.anchoredPosition, targetPos, moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (asLerp)
            {
                menuHolder.localPosition = Vector3.Lerp(menuHolder.localPosition, targetPos, moveSpeed * Time.deltaTime);
            }
            else
            {
                menuHolder.localPosition = Vector3.MoveTowards(menuHolder.localPosition, targetPos, moveSpeed * Time.deltaTime);
            }
        }


        
    }

    public void ToggleMenu() 
    {
        isOpen = !isOpen;
    }

    public void SetMenu(bool state)
    {
        isOpen = state;
    }
}
