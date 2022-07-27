using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using FirebaseWebGL.Examples.Utils;

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
        if(!Application.isEditor)
            FirebaseDatabase.GetJSON(path: "TestPath", gameObject.name, callback: "OnRequestSuccess", fallback: "OnRequestFailed");
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
}
