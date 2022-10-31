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
    private Animator animator;

    //1
    [Header("Screen States")]
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject loginScreen;
    [SerializeField] GameObject signupScreen;

    private List<GameObject> allScreens = new List<GameObject>();

    public MobileViewerController viewerController;

    bool buildingViewer = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        Instance = this;
        currentMenuState = MenuState.STARTSCREEN;
        allScreens = new List<GameObject> { startScreen, loginScreen, signupScreen};
        DisableScreens();
    }

    public void UpdateActiveScreen(MenuState _newState)
    {
        SetAnimatorValues((int)_newState);
        return;

    }

    public void DisableScreens()
    {
        foreach (var item in allScreens)
        {
            item.SetActive(false);
            Debug.Log("Disabled " + item.name);
        }
    }

    public void UpdateActiveScreen(int _stateNumber)
    {
        buildingViewer = (_stateNumber == 5);
        UpdateActiveScreen((MenuState)_stateNumber);

    }

    public void FadeOut()
    {
        animator.Play("FadeOut");
    }

    public void ChangeToBuildScene()
    {
        SceneManager.LoadScene(1);
    }

    public void SetAnimatorValues(int _stateNumber)
    {
        animator.SetInteger("MenuState", _stateNumber);
    }

    public void SpawnObject()
    {
        
        viewerController.InitLoad(buildingViewer);
    }
}
