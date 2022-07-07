using QuinnMeshes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TileSettings 
{
    public Vector2Int size;
    public float perlinZoom;
    public float perlinHeight;
    public float perlinHeightLimit;
    public Vector2 vSize;
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainTile : MonoBehaviour
{
    public Vector2Int tilePosition;
    [Space]
    public bool m_ShowGrid = true;
    public TileSettings settings;

    private Vector3[] vertices;
    internal Vertex[][] points;
    private Mesh mesh;
    private QMesh qMesh;
    internal Vector2 perlinOffset;

    public void GenerateMesh() 
    {
        mesh = new Mesh();
        mesh.name = "Procedural Grid";
        qMesh = new QMesh();

        //vertices
        points = new Vertex[settings.size.x + 1][];
        for (float x = 0; x <= settings.size.x; x++)
        {
            points[(int)x] = new Vertex[settings.size.y + 1];
            for (float z = 0; z <= settings.size.y; z++)
            {
                float y = GetPerlinY(x, z);

                Vector3 vertexPoint = new Vector3(x * settings.vSize.x, y, z * settings.vSize.y);
                points[(int)x][(int)z] = new Vertex(vertexPoint, new Vector2(x/settings.size.x, z/settings.size.y));
            }
        }

        //tris
        for (int x = 0; x < settings.size.x; x++)
        {
            for (int z = 0; z < settings.size.y; z++)
            {
                // bottom left, top left, top right
                Triangle tri = new Triangle(points[x][z], points[x][z + 1], points[x + 1][z + 1]);
                qMesh.triangles.Add(tri);

                // bottom left, top right, bottom left
                Triangle tri1 = new Triangle(points[x + 1][z + 1], points[x + 1][z], points[x][z]);
                qMesh.triangles.Add(tri1);
            }
        }

        mesh = qMesh.ConvertToMesh();
        vertices = mesh.vertices;

        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0; i < vertices.Length; i++) 
        {
            tangents[i] = tangent;
        }
        mesh.tangents = tangents;

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    float GetPerlinY(float x, float z) 
    {
        float y = (Mathf.PerlinNoise(
                        (perlinOffset.x + x + (tilePosition.x * settings.size.x)) * settings.perlinZoom,
                        (perlinOffset.y + z + (tilePosition.y * settings.size.y)) * settings.perlinZoom) - 0.5f)
                        * settings.perlinHeight;

        if (y > settings.perlinHeightLimit) y = settings.perlinHeightLimit;
        else if (y < -settings.perlinHeightLimit) y = -settings.perlinHeightLimit;

        return y;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_ShowGrid)
        {
            m_ShowGrid = false;
            GenerateMesh();
        }

        //if (vertices != null)
        //{
        //    Gizmos.color = Color.black;
        //    for (int i = 0; i < vertices.Length; i++)
        //    {
        //        Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
        //    }
        //}
    }
}
