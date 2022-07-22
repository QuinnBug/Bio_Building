using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUi : MonoBehaviour
{
    [Header("Walls")]
    public ToggleableMenu wallSelectionUi;
    public Transform materialBtnHolder;
    public GameObject materialBtnPrefab;

    [Header("Furniture")]
    public ToggleableMenu furnitureSelectionUi;

    public SelectedType currentOpenUi;

    Material[] materials;
    Sprite[] thumbnails;

    private void Start()
    {
        materials = Resources.LoadAll<Material>("Q_Materials");
        thumbnails = Resources.LoadAll<Sprite>("Q_Thumbnails");
        LoadMaterialChoices();
        LoadFurnitureChoices();
    }

    private void Update()
    {
        if (currentOpenUi != SelectionManager.Instance.SelectedType)
        {
            currentOpenUi = SelectionManager.Instance.SelectedType;

            wallSelectionUi.gameObject.SetActive(currentOpenUi == SelectedType.WALL);
            furnitureSelectionUi.gameObject.SetActive(currentOpenUi == SelectedType.FURNITURE);

            wallSelectionUi.SetMenu(true);
            furnitureSelectionUi.SetMenu(true);
        }
    }

    private void LoadFurnitureChoices()
    {
        
    }

    private void LoadMaterialChoices()
    {
        foreach (Material material in materials)
        {
            GameObject matGo = Instantiate(materialBtnPrefab, materialBtnHolder);
            MaterialBtnScript mbs = matGo.GetComponent<MaterialBtnScript>();
            mbs.mat = material;
            mbs.thumbnail = ThumbnailGenerator.Instance.GetThumbnail(material);
        }
    }

    public void DestroySelected() 
    {
        SelectionManager.Instance.DestroySelected();
    }
}
