using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComponentBtnScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AssignType type;
    [Space]
    public Image display;
    public TextMeshProUGUI title;
    [Space]
    //public Material mat;
    //public Mesh mesh;
    public GameObject prefab;
    public Sprite thumbnail;
    public Sprite defaultThumbnail;

    public void Update()
    {
        if (thumbnail != null) display.sprite = thumbnail;
        else display.sprite = defaultThumbnail;

        if (prefab!=null) title.text = prefab.name;
        else title.text = "No Selection";

    }

    public void ApplyValueToSelected() 
    {
        if(SelectionManager.Instance.selectedObjects.Count > 0) 
        {
            foreach (Selectable selectable in SelectionManager.Instance.selectedObjects)
            {
                selectable.UpdatePrefab(prefab);
                //switch (type)
                //{
                //    case AssignType.MESH:
                //        selectable.data.meshName = mesh.name;
                //        selectable.UpdateMesh();
                //        break;
                //    case AssignType.MATERIAL:
                //        selectable.data.materialName = mat.name;
                //        selectable.UpdateMaterial();
                //        break;
                //
                //    default:
                //        break;
                //}
            }
        }
        else
        {
            //if (type == AssignType.MESH) PlacementManager.Instance.selectedMesh = mesh;
            //if (type == AssignType.MATERIAL) PlacementManager.Instance.selectedMaterial = mat;
            PlacementManager.Instance.selectedPrefab = prefab;
            MeshFilter childMF = prefab.GetComponentInChildren<MeshFilter>();
            PlacementManager.Instance.prefabMesh = childMF.sharedMesh;
            PlacementManager.Instance.prefabMeshOffset = childMF.transform.localPosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (prefab == null) return;

        Debug.Log("Over " + prefab.name);
        SelectionInfoBox.Instance.SetObject(prefab);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (prefab == null) return;
        SelectionInfoBox.Instance.ClearObject(prefab);
    }
}

public enum AssignType 
{
    MESH,
    MATERIAL,
    PREFAB
}
