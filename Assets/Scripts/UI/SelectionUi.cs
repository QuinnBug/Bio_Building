using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionUi : MonoBehaviour
{
    public List<ObjectList> selectionUiList = new List<ObjectList>();
    [Space]
    public ComponentBtnScript selectedPrefabIcon;
    [Space]
    public GameObject btnPrefab;
    public Transform prefabBtnHolder;

    private void Start()
    {
        if (!ResourceManager.Instance.setupComplete) ResourceManager.Instance.Start();

        LoadPrefabChoices();
    }

    private void Update()
    {
        if (selectedPrefabIcon.prefab != PlacementManager.Instance.selectedPrefab)
        {
            selectedPrefabIcon.prefab = PlacementManager.Instance.selectedPrefab;
            selectedPrefabIcon.thumbnail = selectedPrefabIcon.prefab == null ? null : ResourceManager.Instance.GetThumbnail(selectedPrefabIcon.prefab.name);
        }
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
