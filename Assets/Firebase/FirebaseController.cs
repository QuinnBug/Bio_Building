using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using FirebaseWebGL.Examples.Utils;

#if !PLATFORM_WEBGL
using Firebase;
using Firebase.Database;
#endif


public class FirebaseController : MonoBehaviour
{
    public static FirebaseController Instance;
    public FirebaseUser userData;
    private string username { get; set; }
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



        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                break;
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                InitialiseFirebase();        

                break;
            case RuntimePlatform.WebGLPlayer:
                FirebaseWebGL.Scripts.FirebaseBridge.FirebaseDatabase.GetJSON(path: "TestPath", gameObject.name, callback: "OnRequestSuccess", fallback: "OnRequestFailed");
                break;
            default:
                break;
        }
        //if(!Application.isEditor)

        
    }

    private void InitialiseFirebase()
    {
#if !PLATFORM_WEBGL
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if(task.Exception != null)
            {
                Debug.LogError("Cannot Connect");
            }
        });
#endif
    }

    private void OnRequestSuccess(string _data)
    {
        UpdateText( _data);
    }

    private void OnRequestFailed(string _error)
    {
        UpdateText(_error, Color.red);
    }
    public void SignInOrSignOutUser()
    {
        FirebaseAuth.OnAuthStateChanged(gameObject.name, onUserSignedIn: "UserSignedIn", onUserSignedOut: "UserSignedOut");
    }

    private void UserSignedIn(string _data)
    {
        UpdateText(_data);
        userData = StringSerializationAPI.Deserialize(typeof(FirebaseUser), _data) as FirebaseUser;
    }

    private void UserSignedOut(string _error)
    {
        UpdateText(_error, Color.red);
    }

    public void UpdateText(string _text, Color? textColour = null)
    {
        text.color = textColour ?? Color.black;
        text.text = _text;
    }

    //public void postJSON(string )
    //{

    //}
}
