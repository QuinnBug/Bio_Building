using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginCanvasController : MonoBehaviour
{
    public static LoginCanvasController Instance;
    public enum MenuState { STARTSCREEN, LOGIN, SIGNUP}
    private MenuState currentMenuState;

    //1
    [Header("Screen States")]
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject loginScreen;
    [SerializeField] GameObject signupScreen;

    private List<GameObject> allScreens = new List<GameObject>();

    void Start()
    {
        Instance = this;
        currentMenuState = MenuState.STARTSCREEN;
        allScreens = new List<GameObject> { startScreen, loginScreen, signupScreen};
        UpdateActiveScreen((MenuState)0);
    }

    public void UpdateActiveScreen(MenuState _newState)
    {
        foreach (var item in allScreens)
        {
            item.SetActive(false);
        }

        switch (_newState)
        {
            case MenuState.STARTSCREEN:
                startScreen.SetActive(true);
                break;
            case MenuState.LOGIN:
                loginScreen.SetActive(true);
                break;
            case MenuState.SIGNUP:
                signupScreen.SetActive(true);
                break;
            default:
                break;
        }

        currentMenuState = _newState;
    }

    public void UpdateActiveScreen(int _stateNumber)
    {
        UpdateActiveScreen((MenuState)_stateNumber);
    }

    public void ChangeToBuildScene()
    {
        SceneManager.LoadScene(1);
    }
}
