using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThirdPersonIsoCamera : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float xShift;
    public float yShift;
    public float xSpeed = 50.0f;
    public float ySpeed = 150.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 200f;

    float x = 0.0f;
    float y = 0.0f;

    Dictionary<string, GameObject> transparentGOs = new Dictionary<string, GameObject>();

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void Update()
    {
        UpdateTransparencies();
    }

    void LateUpdate()
    {
        if (target)
        {
            UpdatePositionAndRotation();
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    void UpdatePositionAndRotation()
    {
        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;

            y = ClampAngle(y, yMinLimit, yMaxLimit);
        }

        Quaternion rotation = Quaternion.Euler(y + 6, x, 0);
        
        //distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);


        Vector3 negDistance = new Vector3(xShift, yShift, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }

    void UpdateTransparencies()
    {
        RaycastHit[] hits = Physics.RaycastAll(target.position, transform.position - target.position, 1000);

        for(int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.parent.name == "Building")
            {
                if (!((Building)hits[i].transform.parent.gameObject.GetComponent<Building>()).isPlayerInside)
                {
                    hits[i].transform.parent.GetComponent<Transparency>().MakeTransparent(true);


                }

            }
            else
            {
                hits[i].transform.parent.GetComponent<Transparency>().MakeTransparent(true);
                GameObject existingGO;
                if (!transparentGOs.TryGetValue(hits[i].transform.parent.gameObject.name, out existingGO))
                {
                    transparentGOs.Add(hits[i].transform.parent.gameObject.name, hits[i].transform.parent.gameObject);
                }
            }
        }

        var removeList = new List<GameObject>();
        foreach (KeyValuePair<string, GameObject> kvp in transparentGOs)
        {
            bool found = false;
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].transform.parent.name == kvp.Key)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                removeList.Add(kvp.Value);
            }
        }
        foreach (GameObject go in removeList)
        {
            go.GetComponent<Transparency>().MakeTransparent(false);
            transparentGOs.Remove(go.name);
        }

        removeList.Clear();


    }
}