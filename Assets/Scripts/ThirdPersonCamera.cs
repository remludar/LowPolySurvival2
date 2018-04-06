using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera  : MonoBehaviour
{
    CharacterController cc;

    public Transform player;
    Player playerScript;

    float x = 0.0f;
    float y = 0.0f;
    float yMinLimit = -80f;
    float yMaxLimit = 80f;
    float xSpeed = 8.0f;
    float ySpeed = 100.0f;

    float meleDistance = 20.0f;
    float rangeDistance = 5.0f;
    float currentDistance = 20.0f;
    float distanceLerpSpeed = 7.0f;
    float targetDistance = 20.0f;

    float meleLookAtOffsetX = 0.0f;
    float rangeLookAtOffsetX = 1.0f;
    float currentLookAtOffsetX = 0.0f;
    float lookAtOffsetXLerpSpeed = 7.0f;
    float targetLookAtOffsetX = 0.0f;

    Dictionary<string, GameObject> transparentGOs = new Dictionary<string, GameObject>();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cc = GetComponent<CharacterController>();

        playerScript = player.gameObject.GetComponent<Player>();

        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    private void Update()
    {
        UpdateTransparencies();
    }

    private void LateUpdate()
    {
        CheckPlayerState();
        UpdatePosAndRot();
    }

    void UpdatePosAndRot()
    {
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, distanceLerpSpeed * Time.deltaTime);
        currentLookAtOffsetX = Mathf.Lerp(currentLookAtOffsetX, targetLookAtOffsetX, lookAtOffsetXLerpSpeed * Time.deltaTime);

        //set xSpeed relative to distance so its similar regardless of the distance
        xSpeed = (20.0f / currentDistance) * 8.0f;

        //allow camera movement
        if (Input.GetMouseButton(1))
        {
            //horizontal
            x += Input.GetAxis("Mouse X") * xSpeed * currentDistance * Time.deltaTime;

            //vertical only lets the camera go to the ground and back up
            if (!cc.isGrounded)
                y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
            else
            {
                var tempY = Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
                if (tempY < 0)
                    y -= tempY;
            }
        }

        //clamp y
        y = ClampAngle(y, yMinLimit, yMaxLimit);

        //calculate camera position
        var rotation = Quaternion.Euler(y, x, 0);
        var offsetPosition = new Vector3(0, 0, -currentDistance);
        var rotatedOffsetPosition = rotation * offsetPosition;
        var position = rotatedOffsetPosition + player.position;
        var motion = position - transform.position;

        cc.Move(motion);
        transform.LookAt(player.position + (transform.right * currentLookAtOffsetX));
    }

    void CheckPlayerState()
    {
        if (playerScript.state == Player.State.WANDERING)
        {
            targetDistance = meleDistance;
            targetLookAtOffsetX = meleLookAtOffsetX;
        }
        else if (playerScript.state == Player.State.RANGED)
        {
            targetDistance = rangeDistance;
            targetLookAtOffsetX = rangeLookAtOffsetX;
        }
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