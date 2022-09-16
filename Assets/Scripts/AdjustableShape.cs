using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuinnMeshes;

public class AdjustableShape : MonoBehaviour
{
    public float lerpSpeed;

    public List<float> values;
    public List<Vector3> endPoints;
    public Vector3 center;
    public float minimumValue;

    //always lerping towards the local position
    [SerializeField] private List<Vector3> currentPoints;
    private QMesh qMesh;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        currentPoints = new List<Vector3>(endPoints.ToArray());
        qMesh = new QMesh();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 origin = transform.position;

        int pointCount = currentPoints.Count;

        bool redraw = false;

        for (int i = 0; i < pointCount; i++)
        {
            float value = Mathf.Clamp(values[i], minimumValue, 1);
            Vector3 targetPoint = Vector3.Lerp(center, endPoints[i], value);

            if (Vector3.Distance(targetPoint, currentPoints[i]) > 0.05f) redraw = true;

            currentPoints[i] = Vector3.Lerp(currentPoints[i], targetPoint, lerpSpeed * Time.deltaTime);
        }

        if (!redraw) return;

        List<Triangle> triangles = new List<Triangle>();

        Vertex zero = new Vertex(center, Vector2.one);
        for (int i = 0; i < pointCount; i++)
        {
            int j = i + 1;
            if (j >= pointCount) j = 0;

            float value = Mathf.Clamp(values[i], minimumValue, 1);
            Vertex one = new Vertex(currentPoints[i], Vector2.zero);
            Vertex two = new Vertex(currentPoints[j], Vector2.zero);
            triangles.Add(new Triangle(zero, one, two));
        }

        qMesh.triangles = triangles;
        meshFilter.sharedMesh = qMesh.ConvertToMesh(meshFilter.sharedMesh);
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Vector3 point in endPoints)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position + point, 2.5f);
        }

        foreach (Vector3 point in currentPoints)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position + point, 2.5f);
        }
    }
}
