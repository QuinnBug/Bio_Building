using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuinnMeshes 
{
    public class QMeshComponent : MonoBehaviour
    {
        QMesh qMesh;
        MeshFilter mFilter;
        MeshRenderer mRenderer;
        MeshCollider mCollider;

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

        }

        public void SetMesh(QMesh newMesh) 
        {
            qMesh = newMesh;

            mFilter.sharedMesh = qMesh.ConvertToMesh();
            mCollider.sharedMesh = mFilter.sharedMesh;
        }
    }
}

