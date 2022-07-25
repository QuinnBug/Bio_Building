using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseController : MonoBehaviour
{
    public static FirebaseController Instance;

    [DllImport(dllName: "__Internal")]
    public static extern void GetJSON(string path, string objectName, string callback, string fallback);

    public Text text;
   

    private void Start()
    {
        if(FirebaseController.Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        GetJSON(path: "TestPath", gameObject.name, callback: "OnRequestSuccess", fallback: "OnRequestFailed");
    }

    private void OnRequestSuccess(string data)
    {
        text.text = data;
    }

    private void OnRequestFailed(string error)
    {
        text.text = error;
    }
}
