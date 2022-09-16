using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSelectable : MonoBehaviour
{
    public SelectedType type;
}



public enum SelectedType
{
    NONE,
    WALL,
    FLOOR,
    ROOF,
    COLUMN,
}