using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Scripts.FirebaseBridge;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    private FirebaseController firebaseController;

    [Header("Text Holders")]
    [SerializeField] TMP_InputField emailaddress;
    [SerializeField] TMP_InputField password;

    [Header("Login Status")]
    [SerializeField] TextMeshProUGUI invalidDetails;

    // Start is called before the first frame update
    void OnEnable()
    {
        firebaseController = FirebaseController.Instance;
        invalidDetails.gameObject.SetActive(false);
        emailaddress.text = "";
        password.text = "";
    }

    public void loginButtonPress()
    {
        Debug.Log("Checking email '" + emailaddress.text + "' and password '" + password.text+"'");
        checkLoginDetails(emailaddress.text, password.text);
    }

    private void checkLoginDetails(string _emailAddress, string _password)
    {

        FirebaseAuth.SignInWithEmailAndPassword(_emailAddress, _password, gameObject.name, callback: "OnRequestSuccess", fallback: "OnRequestFailed");

    }

    private void OnRequestSuccess(string data)
    {               
        //firebaseController.SetUsername(data);
        SceneManager.LoadScene(0);
    }

    private void OnRequestFailed(string error)
    {
        invalidDetails.gameObject.SetActive(true);
    }
}
