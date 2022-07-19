using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditNode : MonoBehaviour
{
    public bool moveNode;

    public void OnClick()
    {
        Debug.Log("Clicked");

        if (moveNode)
        {
            SelectionManager.Instance.MoveSelectable();
        }
        else
        {
            SelectionManager.Instance.EditSelectable(transform.position);
        }
    }
}
