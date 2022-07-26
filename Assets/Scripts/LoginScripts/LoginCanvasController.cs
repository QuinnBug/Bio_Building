using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LoginCanvasController : MonoBehaviour
{
    public enum MenuState { STARTSCREEN, LOGIN, SIGNUP}
    private MenuState currentMenuState;

    //1
    [Header("Screen States")]
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject loginScreen;
    [SerializeField] GameObject signupScreen;

    private List<GameObject> allScreens = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        currentMenuState = MenuState.STARTSCREEN;
        allScreens = new List<GameObject> { startScreen, loginScreen, signupScreen};
        updateActiveScreen(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateActiveScreen(int newState)
    {
        foreach (var item in allScreens)
        {
            item.SetActive(false);
        }
        allScreens[newState].SetActive(true);

        currentMenuState = (MenuState) newState;
    }
}
