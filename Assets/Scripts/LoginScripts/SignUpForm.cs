using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

public class SignUpForm : AccountFormBase
{
    [System.Serializable]
    public struct SignUpDetails
    {
        //string CurrentRole;
        //string NumberOfYearsInRole;
        //string KeyResponsibilities;

        //string PreviousRoles;
        //string numberOfYearsInRole;
        //string PreviousKeyResponsibilities;

        //string HighestQualification;
        //string Gender;
        //string Age;
        //string ClientBase;
        //string YearsOfExperienceUsingDigitalToolsForBusiness;
        //string YearsOfExperienceUsingDigitalToolsOtherThanBIM;

        //string ToolsPreviouslyUsed;
        //string LevelOfExpertise;

        //string NumberOfYearsExperienceUsingBIM;
        public TextMeshProUGUI key;
        public TextMeshProUGUI value;
    }
    public override void Awake()
    {
        base.Awake();
        textMeshProUGUIs.Add(confirmPasswordField);
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void ConfirmButtonPress()
    {
        base.ConfirmButtonPress();
        if (CheckPasswords()) 
            loginController.checkSignUpDetails(emailAddressField.text, passwordField.text);
        else
            loginController.ChangeWarningText("Passwords don't match.");

    }

    private bool CheckPasswords()
    {
        return passwordField.text == confirmPasswordField.text;
    }

    public override void ReturnButtonPress()
    {
        base.ReturnButtonPress();
        loginCanvasController.UpdateActiveScreen(LoginCanvasController.MenuState.STARTSCREEN);
    }

    public void WriteToJSON()
    {

    }

    [SerializeField]
    public List<SignUpDetails> details = new List<SignUpDetails>();
}
