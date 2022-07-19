using QuinnMeshes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallMeshData 
{
    public int id;
    [Space]
    public Vector3 startPoint;
    public Vector3 endPoint;
    [Space]
    public float height;
    [Space]
    internal Vector3 localStart;
    internal Vector3 localEnd;
    [Space]
    public List<WindowData> windows = new List<WindowData>();

}

public class WallMeshComponent : BaseSelectable
{
    public static Vector3 baseSectionSize = new Vector3(0.25f, 0.25f, 0.05f);

    public WallMeshData data = new WallMeshData();
    internal QMeshComponent qMeshComp;
    internal Outline outline;


    public void SetValues(Vector3 firstPoint, Vector3 secondPoint, float _height, int id) 
    {
        if (firstPoint.x > secondPoint.x)
        {
            data.startPoint = secondPoint;
            data.endPoint = firstPoint;
        }
        else
        {
            data.startPoint = firstPoint;
            data.endPoint = secondPoint;
        }

        data.height = _height;
        data.id = id;
    }

    public Vector3 GetVertexPos(int x, int y, Vector3 sectionSize) 
    {
        Vector3 vPos = data.localStart + new Vector3(x * sectionSize.x, y * sectionSize.y, 0);

        foreach (WindowData window in data.windows)
        {
            Bounds bounds = new Bounds(window.pos, window.size);
            if (bounds.Contains(vPos))
            {
                vPos.x = window.pos.x + (vPos.x > window.pos.x ? -window.size.x : window.size.x);
                //vPos.y = window.pos.y + (vPos.y > window.pos.y ? -window.size.y : window.size.y);
                break;
            }
        }

        return vPos;
    }

    public bool Init()
    {
        type = SelectedType.WALL;

        if (!TryGetComponent(out qMeshComp))
        {
            qMeshComp = gameObject.AddComponent<QMeshComponent>();
        }

        qMeshComp.Init();

        return GenerateMesh();
    }

    bool GenerateMesh() 
    {
        //set position to the center of start/end
        transform.position = data.startPoint;
        //transform.position = Vector3.Lerp(startPoint, endPoint, 0.5f);

        // convert start/end to local positions
        data.localStart = transform.InverseTransformPoint(data.startPoint);
        data.localEnd = transform.InverseTransformPoint(data.endPoint);
        float fullDistance = Vector3.Distance(data.localStart, data.localEnd);

        // calculate how many sections of wall there needs to be - x and y

        Vector2 sectionData = new Vector2();
        Vector2Int sectionCount = new Vector2Int();
        Vector2 overflow = new Vector2();

        // fullDistance / baseSectionSize.x = sectionCount.x and overFlow.x
        sectionData.x = fullDistance / baseSectionSize.x;
        overflow.x = sectionData.x % 1;
        sectionCount.x = (int)(sectionData.x - overflow.x);

        //set something up here to shrink the section size to fit the single small section that we should have
        if (sectionCount.x < 1)
        {
            Destroy(gameObject);
            return false;
        }

        // height / baseSectionSize.y = sectionCount.y and overFlow.y
        sectionData.y = data.height / baseSectionSize.y;
        overflow.y = sectionData.y % 1;
        sectionCount.y = (int)(sectionData.y - overflow.y);
        if (sectionCount.y < 1)
        {
            sectionCount.y = 1;
            overflow.y = 0;
        }

        Vector3 sectionSize = baseSectionSize;
        sectionSize.x += ((overflow.x / sectionData.x) * baseSectionSize.x);
        sectionSize.y += ((overflow.y / sectionData.y) * baseSectionSize.y);

        #region Vertices
        //create vertices from start point to end point
        // bottom left vertex to top right vertex - front
        List<Vertex> frontVertices = new List<Vertex>();
        for (int y = 0; y <= sectionCount.y; y++)
        {
            for (int x = 0; x <= sectionCount.x; x++)
            {
                Vector3 vPos = GetVertexPos(x, y, sectionSize);

                vPos.z = -sectionSize.z / 2;

                Vertex v = new Vertex(vPos,
                    new Vector2(x / sectionCount.x, y / sectionCount.y));
                frontVertices.Add(v);
            }
        }

        // bottom left vertex to top right vertex - back (the x is inverted because it's 180 degrees)
        List<Vertex> backVertices = new List<Vertex>();
        for (int y = 0; y <= sectionCount.y; y++)
        {
            for (int x = sectionCount.x; x >= 0; x--)
            {
                Vector3 vPos = GetVertexPos(x, y, sectionSize);

                vPos.z = sectionSize.z / 2;

                Vertex v = new Vertex(vPos,
                    new Vector2((sectionCount.x - x) / sectionData.x, y / sectionData.y));
                backVertices.Add(v);
            }
        }
        #endregion

        #region Triangles
        //triangles come in pairs to form quads
        // tri = bottom left, top left, top right
        // tri1 = bottom left, top right, bottom right

        int xVerticesCount = sectionCount.x + 1;

        //Debug.Log(sectionCount +" ~ "+ frontVertices.Count + " ~ " + backVertices.Count);

        //front
        for (int y = 0; y < sectionCount.y; y++)
        {
            for (int x = 0; x < sectionCount.x; x++)
            {
                Triangle tri = new Triangle(
                    frontVertices[x + (y * xVerticesCount)],
                    frontVertices[x + ((y + 1) * xVerticesCount)],
                    frontVertices[(x + 1) + ((y + 1) * xVerticesCount)]);
                qMeshComp.qMesh.triangles.Add(tri);

                Triangle tri1 = new Triangle(
                    frontVertices[x + (y * xVerticesCount)],
                    frontVertices[(x + 1) + ((y + 1) * xVerticesCount)],
                    frontVertices[(x + 1) + (y * xVerticesCount)]);
                qMeshComp.qMesh.triangles.Add(tri1);
            }
        }

        //left
        for (int y = 0; y < sectionCount.y; y++)
        {
            Triangle tri = new Triangle(
                backVertices[sectionCount.x + (y * xVerticesCount)],
                backVertices[sectionCount.x + ((y + 1) * xVerticesCount)],
                frontVertices[0 + ((y + 1) * xVerticesCount)]);
            qMeshComp.qMesh.triangles.Add(tri);

            Triangle tri1 = new Triangle(
                backVertices[sectionCount.x + (y * xVerticesCount)],
                frontVertices[0 + ((y + 1) * xVerticesCount)],
                frontVertices[0 + (y * xVerticesCount)]);
            qMeshComp.qMesh.triangles.Add(tri1);
        }

        //back
        for (int y = 0; y < sectionCount.y; y++)
        {
            for (int x = 0; x < sectionCount.x; x++)
            {
                Triangle tri = new Triangle(
                    backVertices[x + (y * xVerticesCount)],
                    backVertices[x + ((y + 1) * xVerticesCount)],
                    backVertices[(x + 1) + ((y + 1) * xVerticesCount)]);
                qMeshComp.qMesh.triangles.Add(tri);

                Triangle tri1 = new Triangle(
                    backVertices[x + (y * xVerticesCount)],
                    backVertices[(x + 1) + ((y + 1) * xVerticesCount)],
                    backVertices[(x + 1) + (y * xVerticesCount)]);
                qMeshComp.qMesh.triangles.Add(tri1);
            }
        }

        //right
        for (int y = 0; y < sectionCount.y; y++)
        {
            Triangle tri = new Triangle(
                    frontVertices[sectionCount.x + (y * xVerticesCount)],
                    frontVertices[sectionCount.x + ((y + 1) * xVerticesCount)],
                    backVertices[0 + ((y + 1) * xVerticesCount)]);
            qMeshComp.qMesh.triangles.Add(tri);

            Triangle tri1 = new Triangle(
                frontVertices[sectionCount.x + (y * xVerticesCount)],
                backVertices[0 + ((y + 1) * xVerticesCount)],
                backVertices[0 + (y * xVerticesCount)]);
            qMeshComp.qMesh.triangles.Add(tri1);
        }

        //top
        for (int x = 0; x < sectionCount.x; x++)
        {
            Triangle tri = new Triangle(
                    frontVertices[(sectionCount.x - x) + (sectionCount.y * xVerticesCount)],
                    frontVertices[(sectionCount.x - (x + 1)) + (sectionCount.y * xVerticesCount)],
                    backVertices[(x + 1) + (sectionCount.y * xVerticesCount)]);
            qMeshComp.qMesh.triangles.Add(tri);

            Triangle tri1 = new Triangle(
                frontVertices[(sectionCount.x - x) + (sectionCount.y * xVerticesCount)],
                backVertices[(x + 1) + (sectionCount.y * xVerticesCount)],
                backVertices[x + (sectionCount.y * xVerticesCount)]);
            qMeshComp.qMesh.triangles.Add(tri1);
        }

        //bottom
        for (int x = 0; x < sectionCount.x; x++)
        {
            Triangle tri = new Triangle(
                    frontVertices[x],
                    frontVertices[x + 1],
                    backVertices[(sectionCount.x - (x + 1))]);
            qMeshComp.qMesh.triangles.Add(tri);

            Triangle tri1 = new Triangle(
                frontVertices[x],
                backVertices[(sectionCount.x - (x + 1))],
                backVertices[(sectionCount.x - x)]);
            qMeshComp.qMesh.triangles.Add(tri1);
        }

        #endregion

        qMeshComp.GenerateMeshFromQ();

        //setRotation
        Vector3 desiredEuler = (data.startPoint - data.endPoint).normalized;
        desiredEuler = Quaternion.LookRotation(desiredEuler, Vector3.up).eulerAngles;
        desiredEuler.y += 90;
        transform.rotation = Quaternion.Euler(desiredEuler);

        return true;
    }

    internal Vector3 GetClosestVertexWorldPos(Vector3 secondPos)
    {
        float shortestDistance = float.PositiveInfinity;
        Vector3 closestVertex = Vector3.positiveInfinity;

        foreach (Vector3 v in qMeshComp.mFilter.sharedMesh.vertices) 
        {
            Vector3 worldVertex = transform.TransformPoint(v);
            if (Vector3.Distance(worldVertex, secondPos) < shortestDistance)
            {
                closestVertex = worldVertex;
                shortestDistance = Vector3.Distance(worldVertex, secondPos);
            }
        }

        return closestVertex;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(data.startPoint, 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(data.endPoint, 0.15f);

    }
}

public struct WindowData 
{
    public Vector2 pos;
    public Vector2 size;
}
