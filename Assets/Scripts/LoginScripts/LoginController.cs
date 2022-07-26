using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Scripts.FirebaseBridge;
using TMPro;

public class LoginController : MonoBehaviour
{
    private FirebaseController firebaseController;

    [Header("Text Holders")]
    [SerializeField] TextMeshProUGUI emailaddress;
    [SerializeField] TextMeshProUGUI password;

    [Header("Screen States")]
    [SerializeField] TextMeshProUGUI invalidDetails;

    // Start is called before the first frame update
    void Awake()
    {
        firebaseController = FirebaseController.Instance;
        invalidDetails.gameObject.SetActive(false);
        emailaddress.text = "";
        password.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loginButtonPress()
    {
        checkLoginDetails(emailaddress.text, password.text);
    }

    private void checkLoginDetails(string _emailAddress, string _password)
    {
        FirebaseAuth.SignInWithEmailAndPassword(_emailAddress, _password, gameObject.name, callback: "OnRequestSuccess", fallback: "OnRequestFailed");
    }

    private void OnRequestSuccess(string data)
    {
        firebaseController.SetUsername(data);
    }

    private void OnRequestFailed(string error)
    {
        invalidDetails.gameObject.SetActive(true);
    }
}
