using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThirdPersonIsoCamera : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float xSpeed = 50.0f;
    public float ySpeed = 150.0f;

    public float yMinLimit = 45f;
    public float yMaxLimit = 80f;

    float targetDistance;

    RaycastHit hitUL;
    RaycastHit hitLL;
    RaycastHit hitLR;
    RaycastHit hitUR;

    bool isHitUL;
    bool isHitLL;
    bool isHitLR;
    bool isHitUR;

    bool isMinHeight;

    float lerpFactor = 1.0f;

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
        targetDistance = 0.0f;
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

    void UpdateTransparencies()
    {
        RaycastHit[] hits = Physics.RaycastAll(target.position, transform.position - target.position, 1000);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.name != "Main Camera" && hits[i].transform.name != "Terrain" && hits[i].transform.name != "Plane")
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

    void UpdatePositionAndRotation()
    {
        #region oldway
        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
        }
        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);


        var offsetPosition = new Vector3(0, 0, -distance);
        var rotatedOffsetPosition = rotation * offsetPosition;
        Vector3 position = rotatedOffsetPosition + target.position;

        transform.rotation = rotation;
        transform.position = position;
        #endregion





    }

    void CheckPosition()
    {
        var z = Camera.main.nearClipPlane;
        var x = Mathf.Tan(Camera.main.fieldOfView / 3.41f) * z;
        var y = x / Camera.main.aspect;

        var upperLeft = transform.rotation * new Vector3(x, -y, z) + transform.position;
        var lowerLeft = transform.rotation * new Vector3(x, y, z) + transform.position;
        var lowerRight = transform.rotation * new Vector3(-x, y, z) + transform.position;
        var upperRight = transform.rotation * new Vector3(-x, -y, z) + transform.position;

        Debug.DrawLine(upperLeft, upperLeft + (transform.position - upperLeft), Color.red);
        Debug.DrawLine(lowerLeft, lowerLeft + (transform.position - lowerLeft), Color.blue);
        Debug.DrawLine(lowerRight, lowerRight + (transform.position - lowerRight), Color.yellow);
        Debug.DrawLine(upperRight, upperRight + (transform.position - upperRight), Color.black);

        isHitUL = Physics.Raycast(upperLeft, target.position - upperLeft, out hitUL, (target.position - upperLeft).magnitude);
        isHitLL = Physics.Raycast(lowerLeft, target.position - lowerLeft, out hitLL, (target.position - lowerLeft).magnitude);
        isHitLR = Physics.Raycast(lowerRight, target.position - lowerRight, out hitLR, (target.position - lowerRight).magnitude);
        isHitUR = Physics.Raycast(upperRight, target.position - upperRight, out hitUR, (target.position - upperRight).magnitude);

        var targetPosition = transform.position;

        if (isHitLL)
        {
            if (isHitLR)
            {
                var averagedPosition = (hitLL.point + hitLR.point) / 2;
                RaycastHit hit;
                if (Physics.Raycast(target.position, averagedPosition - target.position, out hit))
                {
                    //transform.position = hit.point + hit.normal;
                    targetPosition = hit.point + Vector3.up; //hit.normal;
                }
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(target.position, hitLL.point - target.position, out hit))
                {
                    //transform.position = hit.point + hit.normal;
                    targetPosition = hit.point + Vector3.up; //hit.normal;
                }
            }
        }
        else if (isHitLR)
        {
            RaycastHit hit;
            if (Physics.Raycast(target.position, hitLR.point - target.position, out hit))
            {
                //transform.position = hit.point + hit.normal;
                targetPosition = hit.point + Vector3.up; //hit.normal;
            }
        }

        int count = 0;
        while (!Vector3.Equals(transform.position, targetPosition))
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
            //count++;
            ////if (count > 200000)
            //{
            //    transform.position = targetPosition;
            //    return;
            //}
        }
        //somehow find a lerp path to finalTargetPOsition

    }

   

    

}