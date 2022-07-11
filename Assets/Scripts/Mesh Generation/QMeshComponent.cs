using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuinnMeshes 
{
    public class QMeshComponent : MonoBehaviour
    {
        public QMesh qMesh;
        internal MeshFilter mFilter;
        internal MeshRenderer mRenderer;
        internal MeshCollider mCollider;

        public void Init()
        {
            if (TryGetComponent(out mFilter) == false)
            {
                mFilter = gameObject.AddComponent<MeshFilter>();
            }

            if (TryGetComponent(out mCollider) == false)
            {
                mCollider = gameObject.AddComponent<MeshCollider>();
            }

            if (TryGetComponent(out mRenderer) == false)
            {
                mRenderer = gameObject.AddComponent<MeshRenderer>();
            }

            qMesh = new QMesh();
        }

        public void SetMesh(QMesh newMesh) 
        {
            qMesh = newMesh;

            mFilter.sharedMesh = qMesh.ConvertToMesh();
            mCollider.sharedMesh = mFilter.sharedMesh;
        }

        internal void GenerateMeshFromQ()
        {
            Mesh mesh = qMesh.ConvertToMesh();
            mFilter.sharedMesh = mesh;
            mCollider.sharedMesh = mFilter.sharedMesh;
            mCollider.convex = true;
            mRenderer.material = (Material)Resources.Load("Wall_Base");
        }
    }
}

