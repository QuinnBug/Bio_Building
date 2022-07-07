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

        Vector2 sectionCount = new Vector2();
        Vector2 overflow = new Vector2();

        // fullDistance / baseSectionSize.x = sectionCount.x and overFlow.x
        sectionCount.x = fullDistance / baseSectionSize.x;
        overflow.x = sectionCount.x % 1;
        sectionCount.x -= overflow.x;

        // height / baseSectionSize.y = sectionCount.y and overFlow.y
        sectionCount.y = height / baseSectionSize.y;
        overflow.y = sectionCount.y % 1;
        sectionCount.y -= overflow.y;

        Vector3 sectionSize = new Vector3();
        sectionSize.x = baseSectionSize.x + ((overflow.x / sectionCount.x) * baseSectionSize.x);
        sectionSize.y = baseSectionSize.y + ((overflow.y / sectionCount.y) * baseSectionSize.y);

        // for each section set vertices and type (basically wall or window?)
    }
}
