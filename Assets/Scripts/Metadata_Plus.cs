using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Metadata_Plus : MonoBehaviour
{
    [Header("Dictionary Editor")]
    public string key;
    public string value;
    public bool createParameterFromInspector = false;
    [Space]
    public SerializedDictionary<string, string> parameters = new SerializedDictionary<string, string>();

    private void OnValidate()
    {
        if (createParameterFromInspector)
        {
            if (parameters.ContainsKey(key)) parameters.Remove(key);
            parameters.Add(key, value);
            createParameterFromInspector = false;
        }
    }
}
