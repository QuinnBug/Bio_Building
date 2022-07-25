using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentBtnScript : MonoBehaviour
{
    public AssignType type;
    [Space]
    public Image display;
    public TextMeshProUGUI title;
    [Space]
    public Material mat;
    public Mesh mesh;
    public Sprite thumbnail;

    public void Update()
    {
        if(thumbnail != null) display.sprite = thumbnail;

        if (mesh != null || mat != null) title.text = type == AssignType.MATERIAL ? mat.name : mesh.name;
        else title.text = "Invalid";

    }

    public void ApplyValueToSelected() 
    {
        if(SelectionManager.Instance.selectedObjects.Count > 0) 
        {
            foreach (Selectable selectable in SelectionManager.Instance.selectedObjects)
            {
                switch (type)
                {
                    case AssignType.MESH:
                        selectable.data.meshName = mesh.name;
                        selectable.UpdateMesh();
                        break;
                    case AssignType.MATERIAL:
                        selectable.data.materialName = mat.name;
                        selectable.UpdateMaterial();
                        break;

                    default:
                        break;
                }
                
            }
        }
        else
        {
            if (type == AssignType.MESH) PlacementManager.Instance.selectedMesh = mesh;
            if (type == AssignType.MATERIAL) PlacementManager.Instance.selectedMaterial = mat;
        }
    }
}

public enum AssignType 
{
    MESH,
    MATERIAL
}
