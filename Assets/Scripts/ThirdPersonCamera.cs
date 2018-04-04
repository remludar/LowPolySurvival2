﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera  : MonoBehaviour
{
    public Transform player;
    CharacterController cc;
    float x = 0.0f;
    float y = 0.0f;

    float yMinLimit = -80f;
    float yMaxLimit = 80f;

    float xSpeed = 8.0f;
    float ySpeed = 100.0f;
    float distance = 20.0f;

    Dictionary<string, GameObject> transparentGOs = new Dictionary<string, GameObject>();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;


        cc = GetComponent<CharacterController>();
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    private void Update()
    {
        UpdateTransparencies();
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
        }
        y = ClampAngle(y, yMinLimit, yMaxLimit);
        var rotation = Quaternion.Euler(y, x, 0);
        var offsetPosition = new Vector3(0, 0, -distance);
        var rotatedOffsetPosition = rotation * offsetPosition;
        var position = rotatedOffsetPosition + player.position;

        var target = rotation * new Vector3(player.position.x, player.position.y, player.position.z - distance);
        var motion = position - transform.position;

        cc.Move(motion);
        transform.LookAt(player);
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    void UpdateTransparencies()
    {
        RaycastHit[] hits = Physics.RaycastAll(player.position, transform.position - player.position, 1000);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.name != "Main Camera" && hits[i].transform.name != "Terrain" && hits[i].transform.name != "Plane" && hits[i].transform.name != "Player")
            {
                //Debug.Log(hits[i].transform.name);
                //if (hits[i].transform.parent.name == "Building")
                //{
                //    if (!((Building)hits[i].transform.parent.gameObject.GetComponent<Building>()).isPlayerInside)
                //    {
                //        hits[i].transform.parent.GetComponent<Transparency>().MakeTransparent(true);


                //    }

                //}
                //else
                {
                    Debug.Log(hits[i].transform.name);
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
                Debug.Log(hits[j].transform.name);
                if (hits[j].transform.name != "Player")
                {
                    if (hits[j].transform.parent.name == kvp.Key)
                    {
                        found = true;
                        break;
                    }
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