using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Linq;

public class SignUpForm : AccountFormBase
{
    private enum PasswordState { TOOSHORT, DONTMATCH, CORRECT}
    [System.Serializable]
    public class SignUpDetails
    {
        [SerializeField]
        //public Dictionary<string, string> QuestionaireAnswers = new Dictionary<string, string>();
        public List <Dictionary<string, string>> QuestionaireAnswers;
        public object[] objects;
        public string jsonString;
        //public List<SignUpDetail> details = new List<SignUpDetail>();
        public SignUpDetails()
        {
            Dictionary<string, string> TempAnswers = new Dictionary<string, string>();
            GameObject[] detailHolders = GameObject.FindGameObjectsWithTag("Detail");
            //Dictionary<string, string> loadedDetails = new Dictionary<string, string>();

            foreach (var item in detailHolders)
            {
                TempAnswers[AdjustKey(item.FindComponentInChildWithTag<TextMeshProUGUI>("DetailsKey", false).text)] = 
                    CheckValue(item.FindComponentInChildWithTag<TextMeshProUGUI>("DetailsValue", false).text);
                //QuestionaireAnswers.Add( AdjustKey(item.FindComponentInChildWithTag<TextMeshProUGUI>("DetailsKey", false).text),
                //     CheckValue(item.FindComponentInChildWithTag<TextMeshProUGUI>("DetailsValue", false).text));
               
            }
            var list = TempAnswers.Select(p => new Dictionary<string, string>() { { p.Key, p.Value } });

            jsonString = JsonConvert.SerializeObject(list);

            jsonString = jsonString.Replace("},{", ",");
            jsonString = jsonString.Replace("[","");
            jsonString = jsonString.Replace("]","");
        }

        private string AdjustKey(string _invalue)
        {
            string _outValue = _invalue.Trim(new Char[] { ':' });
            return _outValue;
        }

        private string CheckValue(string _invalue)
        {
            string _outValue = (_invalue == "Select" || _invalue == "\u200B") ? "-" : _invalue;
            return _outValue;
        }
    }
    //[System.Serializable]
    //public class SignUpDetail
    //{
    //    public string key;
    //    public string value;

    //    public SignUpDetail(string _key, string _value)
    //    {
    //        key = _key;
    //        value = _value=="Select"? "" : _value;
    //    }
    //}
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
        switch (CheckPasswords())
        {
            case PasswordState.TOOSHORT:
                loginController.ChangeWarningText("Password is too short, please be 6 or more characters");
                break;
            case PasswordState.DONTMATCH:            
                loginController.ChangeWarningText("Passwords don't match.");
                break;
            case PasswordState.CORRECT:
                loginController.checkSignUpDetails(emailAddressField.text, passwordField.text, this);
                break;
            default:
                break;
        }
    }

    private PasswordState CheckPasswords()
    {
        if (passwordField.text.Length < 6) return PasswordState.TOOSHORT;
        if (passwordField.text != confirmPasswordField.text) return PasswordState.DONTMATCH;
        return PasswordState.CORRECT;
    }

    public override void ReturnButtonPress()
    {
        base.ReturnButtonPress();
        loginCanvasController.UpdateActiveScreen(LoginCanvasController.MenuState.STARTSCREEN);
    }

    public string WriteToJSON(string _userID)
    {
        SignUpDetails details = new SignUpDetails();
        return details.jsonString;
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
