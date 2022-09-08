using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class PlaceOnPlane : MonoBehaviour
    {
        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject objectHolder;

        public ARCanvasController canvasController;
        public MobileViewerController viewerController;
        void Awake()
        {
            objectHolder.SetActive(false);
            m_RaycastManager = GetComponent<ARRaycastManager>();
            m_PlaneManager = GetComponent<ARPlaneManager>();
        }

        bool TryGetTouchPosition(out Vector2 touchPosition)
        {
            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
                return true;
            }

            touchPosition = default;
            return false;
        }

        void Update()
        {
            if (!TryGetTouchPosition(out Vector2 touchPosition))
                return;

            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;

                if (objectHolder.activeInHierarchy == false)
                {
                    objectHolder.transform.position = hitPose.position;
                    objectHolder.transform.rotation = hitPose.rotation;
                    objectHolder.SetActive(true);
                    viewerController.LoadByIncrementingObject(0);
                    m_PlaneManager.requestedDetectionMode = PlaneDetectionMode.None;
                    canvasController.SwitchARState(2);
                }
            }
        }

        public void DisableObjectHolder()
        {
            objectHolder.GetComponent<ObjectController>().ClearCurrentObject();
            objectHolder.SetActive(false);
            foreach (var item in m_PlaneManager.trackables)
            {
                item.enabled = true;
            }
            m_PlaneManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
            canvasController.SwitchARState(0);
        }

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        ARRaycastManager m_RaycastManager;
        ARPlaneManager m_PlaneManager;
    }
}
