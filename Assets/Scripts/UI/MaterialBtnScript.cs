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
            switch (SelectionManager.Instance.SelectedType)
            {
                case SelectedType.NONE:
                    break;
                case SelectedType.WALL:
                    WallMeshComponent wmc = (WallMeshComponent)selectable;
                    wmc.data.matName = mat.name;
                    wmc.UpdateMaterial();
                    break;
                case SelectedType.FURNITURE:
                    break;
                default:
                    break;
            }

            MeshRenderer mr = selectable.GetComponent<MeshRenderer>();
            mr.material = mat;
        }
    }
}
