using TMPro;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AccountFormBase), true), CanEditMultipleObjects]
public class AccountFormEditor : Editor
{
    private FormType lastFormType;
    AccountFormBase accountForm;

    void OnEnable()
    {
        accountForm = (AccountFormBase)target;
        lastFormType = accountForm.formType;
    }

    public override void OnInspectorGUI()
    {
        if (lastFormType != accountForm.formType)
        {
            lastFormType = accountForm.formType;
            accountForm.ClearInputFields();
        }
        accountForm.formType = (FormType)EditorGUILayout.EnumPopup("LoginType", accountForm.formType);

        accountForm.emailAddressField = (TMP_InputField)EditorGUILayout.ObjectField("Email Address Field", accountForm.emailAddressField, typeof(TMP_InputField), true);
        accountForm.passwordField = (TMP_InputField)EditorGUILayout.ObjectField("Password Field", accountForm.passwordField, typeof(TMP_InputField), true);

        switch (accountForm.formType)
        {
            case FormType.LOGIN:

                break;
            case FormType.SIGNUP:
                accountForm.confirmPasswordField = (TMP_InputField)EditorGUILayout.ObjectField("Confirm Password Field", accountForm.confirmPasswordField, typeof(TMP_InputField), true);
                break;
            default:
                break;
        }
    }
}