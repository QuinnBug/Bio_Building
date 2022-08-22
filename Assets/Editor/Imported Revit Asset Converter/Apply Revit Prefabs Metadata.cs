using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Reflect;

public class ApplyRevitPrefabsMetadata : EditorWindow
{
    Object myBasePrefab = null;
    string exportedObjectPath = "Resources/FormattedPrefabs/";
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Reflect/Apply Prefab Metadata")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ApplyRevitPrefabsMetadata window = (ApplyRevitPrefabsMetadata)EditorWindow.GetWindow(typeof(ApplyRevitPrefabsMetadata));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Exported object path", EditorStyles.boldLabel);
        exportedObjectPath = EditorGUILayout.TextField(exportedObjectPath);        
        
        GUILayout.Label("Base Revit Export Prefab", EditorStyles.boldLabel);
        myBasePrefab = EditorGUILayout.ObjectField(myBasePrefab, typeof(Object), true);

        if(GUILayout.Button("Apply Metadata"))
        {
            if (exportedObjectPath == "" || exportedObjectPath == null)
            {
                Debug.LogWarning("No exported object path set");
                return;
            }
            if (myBasePrefab == null)
            {
                Debug.LogWarning("No prefab to export set");
                return;
            }
            ApplyMetadata();
        }

    }

    public void ApplyMetadata()
    {
        int debugChildrenCount = 0;
        Debug.Log("ApplyingMetadata");
        string prefabPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(myBasePrefab)); 
        if(prefabPath == null || prefabPath == "")
            prefabPath = AssetDatabase.GetAssetPath(myBasePrefab);

        GameObject basePrefabGameObject;
        try
        {
            basePrefabGameObject = LoadPrefab(prefabPath);
        }
        catch (System.Exception)
        {
            Debug.LogError("Asset Cannot be found at : " + prefabPath);
            return;
        }


        List<GameObject> childPrefabs = new List<GameObject>();
        for (int i = 0; i < basePrefabGameObject.transform.childCount; i++)
        {
            childPrefabs.Add(basePrefabGameObject.transform.GetChild(i).gameObject);
        }
        Debug.Log("child count = " + childPrefabs.Count);
        foreach (var item in childPrefabs)
        {
            string childPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(item));
            childPath = ReformatChildren(LoadPrefab(childPath));
            var childPrefabGameObject = LoadPrefab(childPath);
            //ReformatChildren(childPrefabGameObject);
            CreateAndAddMetadata(childPrefabGameObject,item);
            CreateAndRecentreBounds(childPrefabGameObject);
            SavePrefabData(childPrefabGameObject, childPath);

            debugChildrenCount++;
            if (debugChildrenCount == 50)
                break;
        }
        SavePrefabData(basePrefabGameObject, prefabPath);
        Debug.Log("Metadata Application Complete");
    }        
    void CopyValues(Metadata _from, Metadata _to)
    {
        var json = JsonUtility.ToJson(_from);
        JsonUtility.FromJsonOverwrite(json, _to);
    }

    void SavePrefabData(GameObject _prefab, string _path)
    {
        EditorUtility.SetDirty(_prefab);
        PrefabUtility.SaveAsPrefabAsset(_prefab, _path);
        PrefabUtility.UnloadPrefabContents(_prefab);
    }

    GameObject LoadPrefab(string _prefabPath)
    {
        return PrefabUtility.LoadPrefabContents(_prefabPath);
    }
    string ReformatChildren(GameObject _childPrefab)
    {
        _childPrefab.transform.position = Vector3.zero;
        string localPath;

        localPath = "Assets/" + exportedObjectPath + _childPrefab.name + ".prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);  
        
        if (_childPrefab.transform.childCount > 0)
        {
            PrefabUtility.SaveAsPrefabAsset(_childPrefab, localPath);
        }
        else
        {
            GameObject tempGameObject = new GameObject(_childPrefab.name);
            PrefabUtility.SaveAsPrefabAsset(tempGameObject, localPath);
            GameObject formattedObject = LoadPrefab(localPath);
            Instantiate(_childPrefab, formattedObject.transform);
            SavePrefabData(formattedObject, localPath);
            DestroyImmediate(tempGameObject);
        }
        PrefabUtility.UnloadPrefabContents(_childPrefab);

        return localPath;

    }

    void CreateAndAddMetadata(GameObject _childPrefab, GameObject _childGameObject)
    {
        Metadata[] metadatas;
        try
        {
            metadatas = _childGameObject.GetComponents<Metadata>();
            if (metadatas.Length > 0)
            {
                for (int i = 1; i < metadatas.Length; i++)
                {
                    DestroyImmediate(metadatas[i]);
                }
            }
        }
        catch (System.Exception)
        {
            return;
        }

        if (_childPrefab.GetComponent<Metadata>() != null) return;

        Metadata addedMetadata = metadatas[0];

        Metadata prefabMetadata = (Metadata)_childPrefab.AddComponent(typeof(Metadata));

        CopyValues(addedMetadata, prefabMetadata);
        //DestroyImmediate(addedMetadata);
    }

    void CreateAndRecentreBounds(GameObject _childPrefab)
    {
        //if (_childPrefab.GetComponent<BoxCollider>() != null || _childPrefab.GetComponentInChildren<BoxCollider>() != null) return;
        if (_childPrefab.GetComponentInChildren<MeshRenderer>() == null) return;

        GameObject meshHolder = _childPrefab.GetComponentInChildren<MeshRenderer>().gameObject;

        BoxCollider boxCollider = meshHolder.AddComponent<BoxCollider>();
        Vector3 movementBounds = new Vector3(boxCollider.center.x, -((boxCollider.size.y/2)-boxCollider.center.y ), boxCollider.center.z);

        Vector3 newPosition = meshHolder.transform.position - movementBounds;

        meshHolder.transform.position = newPosition;
    }

}