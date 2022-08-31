using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 2;
    public float zoomOutMax = 5;
    public GameObject objectHolder;
    private bool transition;
    Vector3 directionVelocity;
    private GameObject nextObject;
    private GameObject currentObject;
    private bool shrink;

    public static ObjectController instance;

    Vector3 transformOffset;
    public RectTransform frameRect;
    // Update is called once per frame
    private void Start()
    {
        instance = this;
    }
    void Update()
    {
        if(!transition)
        {
            if (Input.GetMouseButtonDown(0) && frameRect.rect.Contains(Input.mousePosition))
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                zoom(difference * 0.01f);
            }
            else if (Input.GetMouseButton(0))
            {
                directionVelocity = (touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;
                directionVelocity = new Vector3(-directionVelocity.y, directionVelocity.x, directionVelocity.z);
            }
            if(!Input.GetMouseButton(0))
            { 
                directionVelocity = Vector3.Lerp(directionVelocity, (directionVelocity.normalized)/10, 5 * Time.deltaTime);
               
            }

            zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
        else
        {
            TransitionObject();

        }
        objectHolder.transform.Rotate(directionVelocity, Space.World);
        if(currentObject != null)
        {
            //currentObject.transform.localPosition = objectHolder.transform.position - currentObject.GetComponentInChildren<BoxCollider>().center;
        }

    }

    private void TransitionObject()
    {
        if (shrink)
        {
            if (objectHolder.transform.localScale.x > 0.05)
            {
                objectHolder.transform.localScale = Vector3.Lerp(objectHolder.transform.localScale, Vector3.zero, 6 * Time.deltaTime);
            }
            else
            {
                objectHolder.transform.localScale = Vector3.one * 0.01f;
                Destroy(currentObject);
                currentObject = Instantiate(nextObject, objectHolder.transform);
                SetLayerRecursively(currentObject, "LoadedViewerModel");
                //currentObject.layer = LayerMask.NameToLayer("LoadedViewerModel");

                currentObject.GetComponentInChildren<MeshCollider>().gameObject.AddComponent<BoxCollider>();
                
                currentObject.transform.localPosition = objectHolder.transform.localPosition - currentObject.GetComponentInChildren<BoxCollider>().center;
                directionVelocity = new Vector3(Random.Range(-1,1),Random.Range(-1,1), Random.Range(-1,1));
                shrink = false;
            }
        }
        else
        {
            if (objectHolder.transform.localScale.x < 2)
            {
                objectHolder.transform.localScale = Vector3.Lerp(objectHolder.transform.localScale, Vector3.one * 2.5f, 3 * Time.deltaTime);

                //objectHolder.transform.localScale += Vector3.one * 3 * Time.deltaTime;
            }
            else
            {
                transition = false;
            }
        }
    }

    public void ChangeObject(GameObject _nextObject)
    {
        transition = true;
        nextObject = _nextObject;
        shrink = true;
    }

    void zoom(float increment)
    {
        objectHolder.transform.localScale = new Vector3( (Mathf.Clamp(objectHolder.transform.localScale.x -  increment, zoomOutMin, zoomOutMax)),
            (Mathf.Clamp(objectHolder.transform.localScale.y - increment, zoomOutMin, zoomOutMax)),
            (Mathf.Clamp(objectHolder.transform.localScale.z - increment, zoomOutMin, zoomOutMax)));
    }

    void SetLayerRecursively( GameObject obj,  string newLayer )
    {
        obj.layer = LayerMask.NameToLayer(newLayer);

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}