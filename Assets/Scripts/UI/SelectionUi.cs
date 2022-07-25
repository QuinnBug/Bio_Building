using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUi : MonoBehaviour
{
    public ToggleableMenu[] selectionUiList = new ToggleableMenu[4];
    [Space]
    public GameObject btnPrefab;
    public Transform materialBtnHolder;
    public Transform meshBtnHolder;
    [Space]
    public SelectedType currentOpenUi;
    


    Material[] materials;
    Mesh[] meshes;
    Sprite[] thumbnails;

    private void Start()
    {
        materials = Resources.LoadAll<Material>("Q_Materials");
        meshes = Resources.LoadAll<Mesh>("Q_Meshes");
        thumbnails = Resources.LoadAll<Sprite>("Q_Thumbnails");
        LoadMaterialChoices();
        LoadMeshChoices();
    }

    private void Update()
    {
        //when the selection type has changed update the correct ui
        if (currentOpenUi != SelectionManager.Instance.SelectedType)
        {
            currentOpenUi = SelectionManager.Instance.SelectedType;

            int i = 0;
            foreach (ToggleableMenu ui in selectionUiList)
            {
                ui.gameObject.SetActive(currentOpenUi == (SelectedType)i);
                ui.SetMenu(true);
                i++;
            }
        }
    }

    private void LoadMaterialChoices()
    {
        foreach (Material material in materials)
        {
            GameObject btnGo = Instantiate(btnPrefab, materialBtnHolder);
            ComponentBtnScript btn = btnGo.GetComponent<ComponentBtnScript>();
            btn.type = AssignType.MATERIAL;
            btn.mat = material;
            btn.thumbnail = ResourceManager.Instance.GetThumbnail(material.name);
        }
    }

    private void LoadMeshChoices()
    {
        foreach (Mesh mesh in meshes)
        {
            GameObject btnGo = Instantiate(btnPrefab, meshBtnHolder);
            ComponentBtnScript btn = btnGo.GetComponent<ComponentBtnScript>();
            btn.type = AssignType.MESH;
            btn.mesh = mesh;
            btn.thumbnail = ResourceManager.Instance.GetThumbnail(mesh.name);
        }
    }

    public void DestroySelected() 
    {
        SelectionManager.Instance.DestroySelected();
    }
}
