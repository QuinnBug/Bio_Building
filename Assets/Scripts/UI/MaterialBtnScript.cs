using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialBtnScript : MonoBehaviour
{
    public Image display;
    public TextMeshProUGUI title;
    [Space]
    public Material mat;
    public Sprite thumbnail;

    public void Update()
    {
        display.sprite = thumbnail;
        title.text = mat.name;
    }

    public void ApplyMaterialToSelected() 
    {
        foreach (BaseSelectable selectable in SelectionManager.Instance.selectedObjects)
        {
            MeshRenderer mr = selectable.GetComponent<MeshRenderer>();
            mr.material = mat;
        }
    }
}
