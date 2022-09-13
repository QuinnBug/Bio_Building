using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

public class SignUpForm : AccountFormBase
{
    [System.Serializable]
    public class SignUpDetails
    {
        [SerializeField]
        public string UserDataID;
        [SerializeField]
        public List<SignUpDetail> details = new List<SignUpDetail>();
    }
    [System.Serializable]
    public struct SignUpDetail
    {
        public string key;
        public string value;

        public SignUpDetail(string _key, string _value)
        {
            key = _key;
            value = _value=="Select"? "" : _value;
        }
    }
    public override void Awake()
    {
        base.Awake();
        textMeshProUGUIs.Add(confirmPasswordField);
        AddDetailHoldersToDetails();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void ConfirmButtonPress()
    {
        base.ConfirmButtonPress();
        Debug.LogError("1");
        if (CheckPasswords())
        {
            Debug.LogError("2");
            loginController.checkSignUpDetails(emailAddressField.text, passwordField.text, this);
        }

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

    public string WriteToJSON(string _userID)
    {
        details.UserDataID = _userID;
        return JsonUtility.ToJson(details);
    }


    [SerializeField]
    public SignUpDetails details = new SignUpDetails();

    private void AddDetailHoldersToDetails()
    {

        GameObject[] detailHolders = GameObject.FindGameObjectsWithTag("Detail");

        foreach (var item in detailHolders)
        {
            details.details.Add(new SignUpDetail(item.FindComponentInChildWithTag<TextMeshProUGUI>("DetailsKey", false).text,
                item.FindComponentInChildWithTag<TextMeshProUGUI>("DetailsValue", false).text));
        }
    }


}

public static class Helper
{
    public static T[] FindComponentsInChildrenWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
    {
        if (parent == null) { throw new System.ArgumentNullException(); }
        if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }
        List<T> list = new List<T>(parent.GetComponentsInChildren<T>(forceActive));
        if (list.Count == 0) { return null; }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].CompareTag(tag) == false)
            {
                list.RemoveAt(i);
            }
        }
        return list.ToArray();
    }

    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
    {
        if (parent == null) { throw new System.ArgumentNullException(); }
        if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }

        T[] list = parent.GetComponentsInChildren<T>(forceActive);
        foreach (T t in list)
        {
            if (t.CompareTag(tag) == true)
            {
                return t;
            }
        }
        return null;
    }
}
