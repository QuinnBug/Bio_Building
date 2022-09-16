using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
public enum FormType { LOGIN, SIGNUP};

public abstract class AccountFormBase : MonoBehaviour
{
    [HideInInspector]
    public LoginController loginController;
    [HideInInspector]
    public LoginCanvasController loginCanvasController;

    [Header("Form Type")]
    public FormType formType;                
    [Header("Login Fields")]
    public TMP_InputField emailAddressField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
    [HideInInspector]
    public List<TMP_InputField> textMeshProUGUIs = new List<TMP_InputField>();

    public virtual void Awake()
    {
        textMeshProUGUIs.Add(emailAddressField);
        textMeshProUGUIs.Add(passwordField);
    }
    public virtual void OnEnable()
    {        
        loginCanvasController = LoginCanvasController.Instance;
        loginController = LoginController.Instance;
        ResetTMPInputFields();
    }
    public virtual void ClearInputFields()
    {
        emailAddressField = null;
        passwordField = null;
        confirmPasswordField = null;
    }

    public virtual void ResetTMPInputFields()
    {
        foreach (TMP_InputField item in textMeshProUGUIs)
        {
            item.text = "";
        }
    }

    public virtual void ConfirmButtonPress()
    {
        loginController.EnableWarningText(false);
    }

    public virtual void ReturnButtonPress()
    {
        loginController.EnableWarningText(false);
    }
}
