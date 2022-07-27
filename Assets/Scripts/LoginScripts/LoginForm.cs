public class LoginForm : AccountFormBase
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void ConfirmButtonPress()
    {
        base.ConfirmButtonPress();
        loginController.checkLoginDetails(emailAddressField.text, passwordField.text);

    }

    public override void ReturnButtonPress()
    {
        base.ReturnButtonPress();
        loginCanvasController.UpdateActiveScreen(LoginCanvasController.MenuState.STARTSCREEN);
    }
}
