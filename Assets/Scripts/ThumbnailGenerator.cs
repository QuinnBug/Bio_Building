using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ThumbnailGenerator : Singleton<ThumbnailGenerator>
{
    public string thumbnailFilepath;
    public string thumbnailFileExtension;
    public Camera thumbCam;
    public Vector2Int thumbnailSize;

    internal Material[] materials;
    internal Mesh[] meshes;
    [SerializeField] internal Sprite[] thumbnails;

    public MeshRenderer exampleObject;

    private Rect rect;
    private RenderTexture renderTexture;
    private Texture2D screenShot;

    public void Start()
    {
        CreateThumbnails();
        thumbCam.enabled = false;
    }

    public void CreateThumbnails() 
    {
        bool needReload = false;
        materials = Resources.LoadAll<Material>("Q_Materials/");
        meshes = Resources.LoadAll<Mesh>("Q_Meshes/");
        thumbnails = LoadThumbnails(); //"Q_Thumbnails"

        List<string> thumbNames = new List<string>();
        foreach (Sprite thumbnail in thumbnails) {thumbNames.Add(thumbnail.name); }

        foreach (Material mat in materials)
        {
            if (thumbNames.Contains(mat.name + thumbnailFileExtension)) continue;

            GenerateThumbnail(mat, null, mat.name);
            needReload = true;
        }

        //foreach (Mesh mesh in meshes)
        //{
        //    if (thumbNames.Contains(mesh.name + thumbnailFileExtension)) continue;

        //    GenerateThumbnail(materials[0], mesh, mesh.name);
        //}

        if (needReload) Debug.LogWarning("RELOAD TO LOAD ALL THUMBNAILS");
    }

    private Sprite[] LoadThumbnails()
    {
        List<Sprite> sprites = new List<Sprite>();

        foreach (Material mat in materials)
        {
            sprites.Add(LoadNewSprite(mat.name + thumbnailFileExtension));
        }

        return sprites.ToArray();
    }

    private void GenerateThumbnail(Material material, Mesh mesh, string name)
    {
        //this needs access to the resources folder for saving, so it can only be done in editor
#if UNITY_EDITOR
        if(material != null) exampleObject.material = material;

        if (mesh != null) exampleObject.GetComponent<MeshFilter>().sharedMesh = mesh;

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

        byte[] fileData = null;
        fileData = screenShot.EncodeToPNG();

        var f = System.IO.File.Create(filename);
        f.Write(fileData, 0, fileData.Length);
        f.Close();
        Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filename, fileData.Length));

        #region commented out since i don't think webgl would appreciate even the chance of multi threading
        //// create new thread to save the image to file (only operation that can be done in background)
        //new System.Threading.Thread(() =>
        //{
        //    // create file and write optional header with image bytes
        //    var f = System.IO.File.Create(filename);
        //    if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
        //    f.Write(fileData, 0, fileData.Length);
        //    f.Close();
        //    Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filename, fileData.Length));

        //}).Start();
        #endregion
#endif
    }

    public Sprite LoadNewSprite(string fileName, float PixelsPerUnit = 100.0f)
    {
        Texture2D SpriteTexture = LoadTexture(Application.dataPath + "/Resources/Q_Thumbnails/" + fileName + ".png");
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

    public Sprite GetThumbnail(Material mat) 
    {
        Sprite sprite = null;

        foreach (Sprite tn in thumbnails)
        {
            if (tn.name == mat.name + thumbnailFileExtension)
            {
                sprite = tn;
                break;
            }
        }

        if (sprite == null && thumbnails != null) sprite = thumbnails[0];

        return sprite;
    }
}
