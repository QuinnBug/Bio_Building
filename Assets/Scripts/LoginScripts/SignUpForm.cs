public class SignUpForm : AccountFormBase
{
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
}
