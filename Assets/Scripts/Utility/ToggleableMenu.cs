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
    public Vector3 menuStartPos;
    public Vector3 menuEndPos;
    [Header("Options")]
    public float lerpSpeed = 1;
    public bool asLerp = true;
    private bool isOpen;

    private void Start()
    {
        menuHolder.localPosition = menuStartPos;
        buttonImage.localRotation = Quaternion.Euler(startEuler);
    }

    private void Update()
    {
        Quaternion targetRot = Quaternion.Euler(isOpen ? endEuler : startEuler);
        buttonImage.localRotation = Quaternion.Lerp(buttonImage.localRotation, targetRot, lerpSpeed * Time.deltaTime);

        Vector3 targetPos = isOpen ? menuEndPos : menuStartPos;
        if (asLerp)
        {
            menuHolder.localPosition = Vector3.Lerp(menuHolder.localPosition, targetPos, lerpSpeed * Time.deltaTime);
        }
        else 
        {
            menuHolder.localPosition = Vector3.MoveTowards(menuHolder.localPosition, targetPos, lerpSpeed * Time.deltaTime);
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
