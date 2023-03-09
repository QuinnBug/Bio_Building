using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : Singleton<ResourceManager>
{
    public GameObject environment;
    [Space]
    public string thumbnailFilepath;
    public string thumbnailFileExtension;
    public Camera thumbCam;
    public Vector2Int thumbnailSize;

    internal Material[] materials;
    internal Mesh[] meshes;
    [SerializeField] internal GameObject[] prefabs;
    [SerializeField] internal Sprite[] thumbnails;

    public Transform exampleObject;

    private Rect rect;
    private RenderTexture renderTexture;
    private Texture2D screenShot;

    internal bool setupComplete = false;

    public bool skipThumbnails = false;
    public void Start()
    {
        if (setupComplete) return;

        //materials = Resources.LoadAll<Material>("Q_Materials/");
        //meshes = Resources.LoadAll<Mesh>("Q_Meshes/");
        prefabs = Resources.LoadAll<GameObject>("Active_Prefabs/");

        if (skipThumbnails)
        {
            setupComplete = true;
            return;
        }
        CreateThumbnails();
        thumbCam.enabled = false;
        setupComplete = true;
    }

    public Material GetMaterial(string matName) 
    {
        foreach (Material material in materials)
        {
            if (material.name == matName) return material;
        }

        Debug.Log(matName + " Material not found");
        return materials[0];
    }

    public GameObject GetPrefab(string _name)
    {
        foreach (GameObject pf in prefabs)
        {
            if (pf.name == _name) return pf;
        }

        Debug.Log(_name + " -- Prefab not found");
        return prefabs[0];
    }

    public Mesh GetMesh(string meshName)
    {
        foreach (Mesh mesh in meshes)
        {
            if (mesh.name == meshName) return mesh;
        }

        Debug.Log(meshName + " Mesh not found");
        return meshes[0];
    }

    public void CreateThumbnails() 
    {
        environment.SetActive(false);

        bool needReload = false;
        
        thumbnails = LoadThumbnails();

        List<string> thumbNames = new List<string>();
        foreach (Sprite thumbnail in thumbnails) {thumbNames.Add(thumbnail.name); }

        foreach (GameObject obj in prefabs)
        {
            if (thumbNames.Contains(obj.name + thumbnailFileExtension)) continue;

            GenerateThumbnail(obj, obj.name);
            needReload = true;
        }

        if (needReload) Debug.LogWarning("RELOAD TO LOAD ALL THUMBNAILS");

        environment.SetActive(true);
    }

    private Sprite[] LoadThumbnails()
    {
        List<Sprite> sprites = new List<Sprite>();

        foreach (Texture2D thumbnail in Resources.LoadAll<Texture2D>("Q_Thumbnails"))
        {
            //Debug.Log(thumbnail.name);
            //sprites.Add(LoadNewSprite(thumbnail.name));

            Sprite NewSprite = Sprite.Create(thumbnail, new Rect(0, 0, thumbnail.width, thumbnail.height), new Vector2(0, 0), 100.0f);
            NewSprite.name = thumbnail.name;
            sprites.Add(NewSprite);
        }

        return sprites.ToArray();
    }

    private void GenerateThumbnail(Material material, Mesh mesh, string name)
    {
        
//        //this needs access to the resources folder for saving, so it can only be done in editor
////#if UNITY_EDITOR
        

//        if(material != null) exampleObject.material = material;

//        if (mesh != null) exampleObject.GetComponent<MeshFilter>().sharedMesh = mesh;

//        if (renderTexture == null)
//        {
//            rect = new Rect(0, 0, thumbnailSize.x, thumbnailSize.y);
//            renderTexture = new RenderTexture(thumbnailSize.x, thumbnailSize.y, 24);
//            screenShot = new Texture2D(thumbnailSize.x, thumbnailSize.y, TextureFormat.RGB24, false);
//        }

//        thumbCam.targetTexture = renderTexture;
//        thumbCam.Render();

//        RenderTexture.active = renderTexture;
//        screenShot.ReadPixels(rect, 0, 0);

//        thumbCam.targetTexture = null;
//        RenderTexture.active = null;

//        string filename = Application.dataPath + "/" + thumbnailFilepath + "/" + name + thumbnailFileExtension + ".png";
//        //Debug.Log(filename);

//        byte[] fileData = null;
//        fileData = screenShot.EncodeToPNG();

//        var f = System.IO.File.Create(filename);
//        f.Write(fileData, 0, fileData.Length);
//        f.Close();
//        Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filename, fileData.Length));

//        #region commented out since i don't think webgl would appreciate even the chance of multi threading
//        //// create new thread to save the image to file (only operation that can be done in background)
//        //new System.Threading.Thread(() =>
//        //{
//        //    // create file and write optional header with image bytes
//        //    var f = System.IO.File.Create(filename);
//        //    if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
//        //    f.Write(fileData, 0, fileData.Length);
//        //    f.Close();
//        //    Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filename, fileData.Length));

//        //}).Start();
//        #endregion
////#endif
    }

    private void GenerateThumbnail(GameObject prefab, string name)
    {
        //this needs access to the resources folder for saving, so it can only be done in editor
        #if UNITY_EDITOR

        GameObject obj = Instantiate(prefab, exampleObject.transform);
        foreach (Transform item in obj.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = LayerMask.NameToLayer("Thumbnail");
        }

        if (renderTexture == null)
        {
            rect = new Rect(0, 0, thumbnailSize.x, thumbnailSize.y);
            renderTexture = new RenderTexture(thumbnailSize.x, thumbnailSize.y, 24);
            screenShot = new Texture2D(thumbnailSize.x, thumbnailSize.y, TextureFormat.RGB24, false);
        }

        thumbCam.targetTexture = renderTexture;
        thumbCam.Render();

        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);

        thumbCam.targetTexture = null;
        RenderTexture.active = null;

        string filename = Application.dataPath + "/" + thumbnailFilepath + "/" + name + thumbnailFileExtension + ".png";
        //Debug.Log(filename);

        byte[] fileData = screenShot.EncodeToPNG();

        var f = System.IO.File.Create(filename);
        f.Write(fileData, 0, fileData.Length);
        f.Close();
        Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filename, fileData.Length));

        foreach (Transform item in obj.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = 0;
        }

        Destroy(obj);
        #endif
    }

    public Sprite LoadNewSprite(string fileName, float PixelsPerUnit = 100.0f)
    {
        Texture2D SpriteTexture = LoadTexture(Application.dataPath + "/Resources/Thumbnails/" + fileName + ".png");
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
        NewSprite.name = fileName;

        return NewSprite;
    }

    public Texture2D LoadTexture(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           
            if (Tex2D.LoadImage(FileData))           
                return Tex2D;                 
        }
        return null;                     
    }

    public Sprite GetThumbnail(string valueName) 
    {
        Sprite sprite = null;

        foreach (Sprite tn in thumbnails)
        {
            if (tn.name == valueName + thumbnailFileExtension)
            {
                return tn;
            }
        }

        if (sprite == null && thumbnails != null) sprite = thumbnails[0];

        return sprite;
    }
}
