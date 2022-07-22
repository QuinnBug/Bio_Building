using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureComponent : BaseSelectable
{
    public FurnitureData data;
}

[System.Serializable]
public struct FurnitureData 
{
    public Vector3 position;
}
