using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController cc;
    Vector3 motion;
    Quaternion lookRotation;

    GameObject currentlyWielding;
    Dictionary<string, GameObject> inventory;

    public float moveSpeed;
    public float rotateSpeed;
    public float gravity = -9.8f;
    Vector3 lookDirection;
    

    void Start()
    {
        cc = GetComponent<CharacterController>();        
        InitInventory();
        lookDirection = transform.forward;
    }

    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var horizontalRaw = Input.GetAxisRaw("Horizontal");
        var verticalRaw = Input.GetAxisRaw("Vertical");

        var cameraForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
        var cameraRight = Camera.main.transform.right;
        motion = ((cameraRight * horizontal) + (cameraForward * vertical));

        //Fix diagonal speeds but preserve smoothing from GetAxis
        var mag = motion.magnitude;
        mag = Mathf.Clamp(mag, 0.0f, 1.0f);
        motion.Normalize();
        motion *= mag;
        motion = new Vector3(motion.x, motion.y + gravity, motion.z);
        motion *= moveSpeed * Time.deltaTime;

        // Rotation
        lookDirection += (horizontalRaw * cameraRight) + (verticalRaw * cameraForward);
        lookDirection = Vector3.ClampMagnitude(lookDirection, 1);
        lookRotation = Quaternion.LookRotation(lookDirection);

        // Temp input handler
        if (Input.GetMouseButton(0))
        {
            currentlyWielding.GetComponent<Hatchet>().Use();
        }
    }


    void LateUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        cc.Move(motion );
    }

    void InitInventory()
    {
        inventory = new Dictionary<string, GameObject>();

        var hatchetGO = Instantiate(Resources.Load("Prefabs/Hatchet")) as GameObject;
        hatchetGO.name = "Hatchet";
        AddToInventory(hatchetGO);
        Wield(hatchetGO);
    }

    void AddToInventory(GameObject go)
    {
        inventory.Add(go.name, go);
    }

    void Wield(GameObject go)
    {
        currentlyWielding = go;
        go.transform.parent = transform.Find("Right Arm");
    }
}