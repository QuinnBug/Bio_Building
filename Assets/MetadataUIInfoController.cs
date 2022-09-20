using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MetadataUIInfoController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI key;    
    [SerializeField]
    TextMeshProUGUI value;

    public void SetValues(string _key, string _value)
    {
        key.text = _key;
        value.text = _value;
    }
}
