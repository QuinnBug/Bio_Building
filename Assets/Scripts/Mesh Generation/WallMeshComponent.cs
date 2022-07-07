using QuinnMeshes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMeshComponent : MonoBehaviour
{
    public static Vector3 baseSectionSize = new Vector3(1.0f, 1.0f, 0.2f);
    [Space]
    public Vector3 startPoint;
    public Vector3 endPoint;
    [Space]
    public float height;
    [Space]
    public Vector3 localStart;
    public Vector3 localEnd;

    internal QMeshComponent qMeshComp;

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    public void Init()
    {
        if (!TryGetComponent(out qMeshComp))
        {
            qMeshComp = gameObject.AddComponent<QMeshComponent>();
        }

        qMeshComp.Init();

        // set position to the center of start/end
        transform.position = Vector3.Lerp(startPoint, endPoint, 0.5f);
        // convert start/end to local positions
        localStart = transform.InverseTransformPoint(startPoint);
        localEnd = transform.InverseTransformPoint(endPoint);
        float fullDistance = Vector3.Distance(localStart, localEnd);

        // calculate how many sections of wall there needs to be - x and y

        Vector2 sectionData = new Vector2();
        Vector2Int sectionCount = new Vector2Int();
        Vector2 overflow = new Vector2();

        // fullDistance / baseSectionSize.x = sectionCount.x and overFlow.x
        sectionData.x = fullDistance / baseSectionSize.x;
        overflow.x = sectionData.x % 1;
        sectionCount.x = (int)(sectionData.x - overflow.x);

        // height / baseSectionSize.y = sectionCount.y and overFlow.y
        sectionData.y = height / baseSectionSize.y;
        overflow.y = sectionData.y % 1;
        sectionCount.y = (int)(sectionData.y - overflow.y);

        Vector3 sectionSize = baseSectionSize;
        sectionSize.x += ((overflow.x / sectionData.x) * baseSectionSize.x);
        sectionSize.y += ((overflow.y / sectionData.y) * baseSectionSize.y);

        // bottom left vertex to top right vertex - front
        List<Vertex> frontVertices = new List<Vertex>();
        for (int y = 0 ; y <= sectionData.y; y++)
        {
            for (int x = 0; x <= sectionData.x; x++)
            {
                Vertex v = new Vertex(
                    localStart + new Vector3(x * sectionSize.x, y * sectionSize.y, sectionSize.z/2),
                    new Vector2(x / sectionCount.x, y / sectionCount.y));
                frontVertices.Add(v);
            }
        }

        // bottom left vertex to top right vertex - back (the x is inverted because it's 180 degrees)
        List<Vertex> backVertices = new List<Vertex>();
        for (int y = 0; y <= sectionData.y; y++)
        {
            for (int x = (int)sectionData.x; x >= 0; x--)
            {
                Vertex v = new Vertex(
                    new Vector3(x * sectionSize.x, y * sectionSize.y, -sectionSize.z/2),
                    new Vector2(x / sectionData.x, y / sectionData.y));
                backVertices.Add(v);
            }
        }

        #region Triangles
        //triangles come in pairs to form quads
        // tri = bottom left, top left, top right
        // tri1 = bottom left, top right, bottom right

        int maxX = sectionCount.x - 1;
        int maxY = sectionCount.y - 1;

        //front
        for (int y = 0; y < sectionData.y; y++)
        {
            for (int x = 0; x < sectionData.x; x++)
            {
                Triangle tri = new Triangle(
                    frontVertices[x + (y * sectionCount.x)],
                    frontVertices[x + ((y + 1) * sectionCount.x)],
                    frontVertices[(x + 1) + ((y + 1) * sectionCount.x)]);
                qMeshComp.qMesh.triangles.Add(tri);

                Triangle tri1 = new Triangle(
                    frontVertices[x + (y * sectionCount.x)],
                    frontVertices[(x + 1) + ((y + 1) * sectionCount.x)],
                    frontVertices[(x + 1) + (y * sectionCount.x)]);
                qMeshComp.qMesh.triangles.Add(tri1);
            }
        }

        //left
        for (int y = 0; y < sectionData.y; y++)
        {
            Triangle tri = new Triangle(
                backVertices[maxX + (y * sectionCount.x)],
                backVertices[maxX + ((y + 1) * sectionCount.x)],
                frontVertices[0 + ((y + 1) * sectionCount.x)]);
            qMeshComp.qMesh.triangles.Add(tri);

            Triangle tri1 = new Triangle(
                backVertices[maxX + (y * sectionCount.x)],
                frontVertices[0 + ((y + 1) * sectionCount.x)],
                frontVertices[0 + (y * sectionCount.x)]);
            qMeshComp.qMesh.triangles.Add(tri1);
        }

        //back
        for (int y = 0; y < sectionData.y; y++)
        {
            for (int x = 0; x < sectionData.x; x++)
            {
                Triangle tri = new Triangle(
                    backVertices[x + (y * sectionCount.x)],
                    backVertices[x + ((y + 1) * sectionCount.x)],
                    backVertices[(x + 1) + ((y + 1) * sectionCount.x)]);
                qMeshComp.qMesh.triangles.Add(tri);

                Triangle tri1 = new Triangle(
                    backVertices[x + (y * sectionCount.x)],
                    backVertices[(x + 1) + ((y + 1) * sectionCount.x)],
                    backVertices[(x + 1) + (y * sectionCount.x)]);
                qMeshComp.qMesh.triangles.Add(tri1);
            }
        }

        //right
        for (int y = 0; y < sectionData.y; y++)
        {
            Triangle tri = new Triangle(
                    frontVertices[maxX + (y * sectionCount.x)],
                    frontVertices[maxX + ((y + 1) * sectionCount.x)],
                    backVertices[0 + ((y + 1) * sectionCount.x)]);
            qMeshComp.qMesh.triangles.Add(tri);

            Triangle tri1 = new Triangle(
                frontVertices[maxX + (y * sectionCount.x)],
                backVertices[0 + ((y + 1) * sectionCount.x)],
                backVertices[0 + (y * sectionCount.x)]);
            qMeshComp.qMesh.triangles.Add(tri1);
        }

        //top
        for (int x = 0; x < sectionData.x; x++)
        {
            Triangle tri = new Triangle(
                    frontVertices[(maxX - x) + (sectionCount.y * sectionCount.x)],
                    frontVertices[(maxX - (x+1)) + (sectionCount.y * sectionCount.x)],
                    backVertices[(x+1) + (sectionCount.y * sectionCount.x)]); 
            qMeshComp.qMesh.triangles.Add(tri);

            Triangle tri1 = new Triangle(
                frontVertices[(maxX - x) + (sectionCount.y * sectionCount.x)],
                backVertices[(x + 1) + (sectionCount.y * sectionCount.x)],
                backVertices[x + (sectionCount.y * sectionCount.x)]);
            qMeshComp.qMesh.triangles.Add(tri1);
        }

        //bottom
        for (int x = 0; x < sectionData.x; x++)
        {
            Triangle tri = new Triangle(
                    frontVertices[x + 0],
                    frontVertices[x+1 + 0],
                    backVertices[(maxX - (x+1)) + 0]);
            qMeshComp.qMesh.triangles.Add(tri);

            Triangle tri1 = new Triangle(
                frontVertices[(maxX - x) + 0],
                backVertices[(maxX - (x + 1)) + 0],
                backVertices[(maxX - x) + 0]);
            qMeshComp.qMesh.triangles.Add(tri1);
        }

        #endregion
    }
}
