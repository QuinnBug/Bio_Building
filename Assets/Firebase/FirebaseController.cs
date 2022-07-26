using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using FirebaseWebGL.Scripts.FirebaseBridge;

public class FirebaseController : MonoBehaviour
{
    public static FirebaseController Instance;
    private string username;
    //[DllImport(dllName: "__Internal")]
    //public static extern void GetJSON(string path, string objectName, string callback, string fallback);

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
#if UNITY_WEBGL
        if(!Application.isEditor)
            FirebaseDatabase.GetJSON(path: "TestPath", gameObject.name, callback: "OnRequestSuccess", fallback: "OnRequestFailed");
#endif
    }

    private void OnRequestSuccess(string _data)
    {
        text.text = _data;
    }

    private void OnRequestFailed(string _error)
    {
        text.text = _error;
    }

    public void SetUsername(string _username)
    {
        username = _username;
    }
}
