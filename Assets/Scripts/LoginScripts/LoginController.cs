using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FirebaseWebGL.Scripts.FirebaseBridge;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    private string filePath = "/SavedDetails";
    public static LoginController Instance;
    private LoginCanvasController loginCanvasController;

    [Header("Login Status")]
    [SerializeField] TextMeshProUGUI warningText;

    UnityEvent m_LoginSuccessEvent;
    UnityEvent m_SignUpSuccessEvent;

    SignUpForm signUpForm;

    private void Start()
    {
        Instance = this;
        loginCanvasController = LoginCanvasController.Instance;
        if (m_LoginSuccessEvent == null)
            m_LoginSuccessEvent = new UnityEvent();

        m_LoginSuccessEvent.AddListener(OnLoginSuccess);

        if (m_SignUpSuccessEvent == null)
            m_SignUpSuccessEvent = new UnityEvent();

        m_SignUpSuccessEvent.AddListener(OnSignUpSuccess);
    }
    void OnEnable()
    {
        EnableWarningText(false);
    }

    /// <summary>
    /// Checks the users email and password for if an account is already created and if the login data is correct
    /// </summary>
    public void checkLoginDetails(string _emailAddress, string _password)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                OnLoginSuccess();
                break;
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                Debug.Log("1");
                StartCoroutine(routine: LoginCoroutine(_emailAddress, _password));
                //Firebase.Auth.FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(_emailAddress, _password).ContinueWith(task =>
                //{
                //    Debug.Log("2");
                //    if (task.Exception != null)
                //    {
                //        Debug.Log("3");
                //        OnLoginFailed(task.Exception.ToString());
                //    }
                //    else
                //    {
                //        Debug.Log("4");
                //        OnLoginSuccess(task.Result.ToString());
                //    }
                //});

                break;
            case RuntimePlatform.WebGLPlayer:
                FirebaseAuth.SignInWithEmailAndPassword(_emailAddress, _password, gameObject.name, callback: "OnLoginSuccess", fallback: "OnLoginFailed");
                break;
            default:
                break;
        }
    }



    /// <summary>
    /// Checks the users email and password for if an account is already created and creates one if there isn't already
    /// </summary>
    public void checkSignUpDetails(string _emailAddress, string _password, SignUpForm _signUpForm)
    {
        signUpForm = _signUpForm;
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                OnSignUpSuccess();
                break;
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                StartCoroutine(routine: SignUpCoroutine(_emailAddress, _password));
                //Firebase.Auth.FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(_emailAddress, _password);//.ContinueWith(task =>
                //{
                //    if (task.Exception != null)
                //    {
                //        OnLoginFailed(task.Exception.ToString());
                //    }
                //    else
                //    {
                //        OnLoginSuccess();
                //    }
                //});

                break;
            case RuntimePlatform.WebGLPlayer:
                FirebaseAuth.CreateUserWithEmailAndPassword(_emailAddress, _password, gameObject.name, callback: "OnSignUpSuccess", fallback: "OnLoginFailed");
                break;
            default:
                break;
        }
    }

    private IEnumerator LoginCoroutine(string _email, string _password)
    {
#if !PLATFORM_WEBGL
        var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        var loginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if(loginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Login Failed with {loginTask.Exception}");
            OnLoginFailed(loginTask.Exception.Message);
        }
        else
        {
            Debug.Log(message: $"Login Succeeded with {loginTask.Result}");
            m_LoginSuccessEvent.Invoke();
        }
#else
        yield return null;
#endif
    }

    private IEnumerator SignUpCoroutine(string _email, string _password)
    {
#if !PLATFORM_WEBGL
        var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        var loginTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Login Failed with {loginTask.Exception}");
            OnLoginFailed(loginTask.Exception.Message);
        }
        else
        {
            Debug.Log(message: $"Login Succeeded with {loginTask.Result}");
            m_SignUpSuccessEvent.Invoke();
        }
#else
        yield return null;
#endif
    }

    /// <summary>
    /// Allows a user to build without having to login in; however they won't have the ability to save
    /// </summary>
    public void anonymousSignin()
    {
        //if(!Application.isEditor)
        //    FirebaseAuth.SignInAnonymously(gameObject.name, callback: "OnLoginSuccess", fallback: "OnLoginFailed");
        //else
            loginCanvasController.SetAnimatorValues(3);
    }
    /// <summary>
    /// Sets the current users data in the FirebaseController if login is successful along with changes the scene
    /// </summary>
    void OnLoginSuccess(/*string _data = null*/)
    {        

        if(Application.platform == RuntimePlatform.WebGLPlayer)
            FirebaseController.Instance.SignInOrSignOutUser();
        Debug.Log("5");
        loginCanvasController.SetAnimatorValues(3);
        Debug.Log("6");
    }

    private void OnSignUpSuccess()
    {
        if(Application.platform == RuntimePlatform.WebGLPlayer)
            FirebaseController.Instance.SignInOrSignOutUser();

        string saveStr;
        // saveStr = Application.isEditor? signUpForm.WriteToJSON("TestString"): signUpForm.WriteToJSON(FirebaseController.Instance.userData.displayName);
        saveStr = signUpForm.WriteToJSON("TestString");
        if (Application.isEditor)
        {
            if (System.IO.File.Exists(Application.dataPath + filePath + ".json"))
            {
                //overwriting file
            }
            else
            {
                //create and close the file, ready to be written to.
                System.IO.File.Create(Application.dataPath + filePath + ".json").Close();
            }

            System.IO.File.WriteAllText(Application.dataPath + filePath + ".json", saveStr);
        }
        else
        {
                FirebaseDatabase.PushJSON(FirebaseController.Instance.userData.uid, saveStr,
                    gameObject.name, callback: "OnWriteToJSONSuccess", fallback: "OnWriteToJSONFailed");
        }
        
        //SaveManager.Instance.UpdateJson(saveStr);


        //loginCanvasController.SetAnimatorValues(3);
        Debug.Log("6");
    }
    /// <summary>
    /// Outputs a warning message for if a users login/signup was unsuccessful
    /// </summary>
    private void OnLoginFailed(string _error)
    {
        ChangeWarningText(_error);
    }

    private void SignOut()
    {

    }

    private void OnWriteToJSONSuccess(string _data)
    {
        FirebaseController.Instance.UpdateText(_data);
        //JSON Write Success
    }

    private void OnWriteToJSONFailed(string _error)
    {
        FirebaseController.Instance.UpdateText(_error, Color.red);
        //JSON Write Failed
    }

    /// <summary>
    /// Changes the display warning message above the login details
    /// </summary>
    public void ChangeWarningText(string _warning)
    {        
        warningText.text = _warning;        
        EnableWarningText(true);
    }
    /// <summary>
    /// Enables or Disables The warning message
    /// </summary>
    public void EnableWarningText(bool _enabled)
    {
        warningText.gameObject.SetActive(_enabled);
    }

#region LegacyCode
    //private void OnSignUpSuccess(string _data)
    //{
    //    firebaseController.SignInOrSignOutUser();
    //    SceneManager.LoadScene(1);
    //}

    //private void OnSignUpFailed(string _error)
    //{
    //    warningText.text = "Email Already In Use";
    //    warningText.gameObject.SetActive(true);
    //    FirebaseController.Instance.UpdateText(_error, Color.red);
    //}
#endregion
}

