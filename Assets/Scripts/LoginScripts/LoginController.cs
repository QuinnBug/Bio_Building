using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Scripts.FirebaseBridge;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    public static LoginController Instance;

    [Header("Login Status")]
    [SerializeField] TextMeshProUGUI warningText;

    private void Start()
    {
        Instance = this;        
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
        FirebaseAuth.SignInWithEmailAndPassword(_emailAddress, _password, gameObject.name, callback: "OnLoginSuccess", fallback: "OnLoginFailed");
    }
    /// <summary>
    /// Checks the users email and password for if an account is already created and creates one if there isn't already
    /// </summary>
    public void checkSignUpDetails(string _emailAddress, string _password)
    {
        FirebaseAuth.CreateUserWithEmailAndPassword(_emailAddress, _password, gameObject.name, callback: "OnLoginSuccess", fallback: "OnLoginFailed");
    }
    /// <summary>
    /// Allows a user to build without having to login in; however they won't have the ability to save
    /// </summary>
    public void anonymousSignin()
    {
        if(!Application.isEditor)
            FirebaseAuth.SignInAnonymously(gameObject.name, callback: "OnLoginSuccess", fallback: "OnLoginFailed");
        else
            SceneManager.LoadScene(1);
    }
    /// <summary>
    /// Sets the current users data in the FirebaseController if login is successful along with changes the scene
    /// </summary>
    private void OnLoginSuccess(string _data)
    {
        FirebaseController.Instance.SignInOrSignOutUser();
        SceneManager.LoadScene(1);
    }
    /// <summary>
    /// Outputs a warning message for if a users login/signup was unsuccessful
    /// </summary>
    private void OnLoginFailed(string _error)
    {
        ChangeWarningText(_error);
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

