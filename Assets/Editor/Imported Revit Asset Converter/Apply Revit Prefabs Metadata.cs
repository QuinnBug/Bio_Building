using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Reflect;

public class ApplyRevitPrefabsMetadata : EditorWindow
{
    Object myBasePrefab = null;
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

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
        GUILayout.Label("Base Revit Export Prefab", EditorStyles.boldLabel);
        myBasePrefab = EditorGUILayout.ObjectField(myBasePrefab, typeof(Object), true);

        if(GUILayout.Button("Apply Metadata"))
        {
            ApplyMetadata();
        }

    }

    public void ApplyMetadata()
    {
        Debug.Log("ApplyingMetadata");
        GameObject prefabToCheck = ((GameObject)myBasePrefab);
        List<Object> childPrefabs = new List<Object>();
        for (int i = 0; i < prefabToCheck.transform.childCount; i++)
        {
            childPrefabs.Add(prefabToCheck.transform.GetChild(i).gameObject);
        }
        Debug.Log("child count = " + childPrefabs.Count);
        foreach (var item in childPrefabs)
        {
            GameObject itemGameobject = ((GameObject)item);
            if(itemGameobject.GetComponent<Metadata>() == null)continue;
            Metadata addedMetadata = itemGameobject.GetComponent<Metadata>();
            Debug.Log("Applying " + item.name+ " " + PrefabUtility.IsAddedComponentOverride(itemGameobject.GetComponent<Metadata>()));
            //foreach (var componentsToCheck in PrefabUtility.GetAddedComponents(item))
            //{
            //    if (componentsToCheck.GetType() == typeof(Metadata)) continue;
            //}
            //if(PrefabUtility.GetAddedComponents)
            //PrefabUtility.IsAddedComponentOverride(item.GetComponent<Metadata>());
            //PrefabUtility.ApplyAddedComponent(item.GetComponent<Metadata>(), AssetDatabase.GetAssetPath(item), InteractionMode.AutomatedAction);
            //Debug.Log("1");
            string path = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabInstanceHandle(item));
            Debug.Log(path);
            var go = PrefabUtility.LoadPrefabContents(path);
            go.AddComponent(typeof(Metadata));
            go.GetComponent<Metadata>().Equals(addedMetadata);
            EditorUtility.SetDirty(go);
            PrefabUtility.SaveAsPrefabAsset(go, path);
            PrefabUtility.UnloadPrefabContents(go);
            //List<ObjectOverride> overrides = new List<ObjectOverride>();
            //overrides = PrefabUtility.get(item, true);
            //foreach (var rides in overrides)
            //{
            //    Debug.Log("2");
            //    Debug.Log(rides.GetAssetObject)
            //    PrefabUtility.ApplyPropertyOverride(rides, )

            //}
            //Debug.Log("3");

        }
        //for (int i = 0; i < childPrefabs.Count; i++)
        //{

        //    if (PrefabUtility.IsPartOfRegularPrefab(childPrefabs[i]))
        //    {
        //        EditorUtility.SetDirty(childPrefabs[i]);
        //        PrefabUtility.ApplyPrefabInstance(childPrefabs[i].gameObject, InteractionMode.AutomatedAction);
        //    }
        //}
    }
}