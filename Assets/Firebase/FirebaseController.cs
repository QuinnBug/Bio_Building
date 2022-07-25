using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FirebaseController : MonoBehaviour
{
    [DllImport(dllName: "__Internal")]
    public static extern void GetJSON(string path, string objectName, string callback, string fallback);

    private void Start()
    {
        GetJSON(path: "TestPath", gameObject.name, callback: "OnRequestSuccess", fallback: "OnRequestFailed");
    }

    private void OnRequestSuccess(string data)
    {

    }

    private void OnRequestFailed(string error)
    {

    }
}
