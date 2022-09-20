using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugCanvasController : MonoBehaviour
{
    public static DebugCanvasController Instance;
    // Start is called before the first frame update
    void Start()
    {
        if (DebugCanvasController.Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
