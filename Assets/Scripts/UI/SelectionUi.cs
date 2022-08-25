using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUi : MonoBehaviour
{
    public List<ObjectList> selectionUiList = new List<ObjectList>();
    [Space]
    public ComponentBtnScript selectedMeshIcon;
    public ComponentBtnScript selectedMatIcon;
    [Space]
    public GameObject btnPrefab;
    public Transform materialBtnHolder;
    public Transform prefabBtnHolder;
    [Space]
    public SelectedType currentSelectedType = SelectedType.COLUMN;

    private void Start()
    {
        if (!ResourceManager.Instance.setupComplete) ResourceManager.Instance.Start();

        LoadPrefabChoices();
    }

    private void Update()
    {
        //when the selection type has changed update the correct ui
        if (currentSelectedType != SelectionManager.Instance.SelectedType)
        {
            currentSelectedType = SelectionManager.Instance.SelectedType;

            foreach (ObjectList uiObjects in selectionUiList)
            {
                foreach (GameObject obj in uiObjects.objects)
                {
                    obj.SetActive(false);
                }
            }

            foreach (GameObject obj in selectionUiList[(int)currentSelectedType].objects)
            {
                obj.SetActive(true);
            }
        }

        //Need to set up the display

        //if (selectedMeshIcon.mesh != PlacementManager.Instance.selectedMesh)
        //{
        //    selectedMeshIcon.mesh = PlacementManager.Instance.selectedMesh;
        //    selectedMeshIcon.thumbnail = selectedMeshIcon.mesh == null ? null : ResourceManager.Instance.GetThumbnail(selectedMeshIcon.mesh.name);

        //}

        //if(selectedMatIcon.mat != PlacementManager.Instance.selectedMaterial) 
        //{
        //    selectedMatIcon.mat = PlacementManager.Instance.selectedMaterial;
        //    selectedMatIcon.thumbnail = selectedMatIcon.mat == null ? null : ResourceManager.Instance.GetThumbnail(selectedMatIcon.mat.name);
        //}
    }

    private void LoadMaterialChoices()
    {
        //foreach (Material material in ResourceManager.Instance.materials)
        //{
        //    GameObject btnGo = Instantiate(btnPrefab, materialBtnHolder);
        //    ComponentBtnScript btn = btnGo.GetComponent<ComponentBtnScript>();
        //    btn.type = AssignType.MATERIAL;
        //    btn.mat = material;
        //    btn.thumbnail = ResourceManager.Instance.GetThumbnail(material.name);
        //}
    }

    private void LoadMeshChoices()
    {
        //foreach (Mesh mesh in ResourceManager.Instance.meshes)
        //{
        //    GameObject btnGo = Instantiate(btnPrefab, meshBtnHolder);
        //    ComponentBtnScript btn = btnGo.GetComponent<ComponentBtnScript>();
        //    btn.type = AssignType.MESH;
        //    btn.mesh = mesh;
        //    btn.thumbnail = ResourceManager.Instance.GetThumbnail(mesh.name);
        //}
    }

    private void LoadPrefabChoices() 
    {
        foreach (GameObject prefab in ResourceManager.Instance.prefabs)
        {
            GameObject btnGo = Instantiate(btnPrefab, prefabBtnHolder);
            ComponentBtnScript btn = btnGo.GetComponent<ComponentBtnScript>();
            btn.type = AssignType.PREFAB;
            btn.prefab = prefab;
            btn.thumbnail = ResourceManager.Instance.GetThumbnail(prefab.name);
        }
    }

    public void DestroySelected() 
    {
        SelectionManager.Instance.DestroySelected();
    }
}

[System.Serializable]
public class ObjectList 
{
    public List<GameObject> objects = new List<GameObject>();
}
