using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController cc;
    Vector3 motion;
    Quaternion lookRotation;

    InventoryItem currentlyWielding;
    Dictionary<string, InventoryItem> inventory;

    public float moveSpeed;
    public float rotateSpeed;
    public float gravity = -9.8f;
    Vector3 lookDirection;

    public enum State { WANDERING, MELE_TARGET, RANGED, RANGED_TARGET };
    public State state = State.WANDERING;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        inventory = new Dictionary<string, InventoryItem>();
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
        if (Input.GetMouseButtonDown(0))
        {
            currentlyWielding.Use();
        }

        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            state = State.WANDERING;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            state = State.RANGED;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        cc.Move(motion);

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (hit.transform.parent != null)
        {
            if (hit.transform.parent.tag == "InventoryItem")
            {
                var hitGO = hit.transform.parent.gameObject;
                var rb = hitGO.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                hitGO.transform.rotation = Quaternion.identity;
                inventory.Add(hitGO.name, hitGO.GetComponent<InventoryItem>());
                Wield(hit.transform.parent.gameObject.GetComponent<InventoryItem>());
            }
        }
    }



    void AddToInventory(InventoryItem item)
    {
        inventory.Add(item.gameObject.name, item);
    }

    void Wield(InventoryItem item)
    {
        currentlyWielding = item;
        var rightHand = GameObject.Find("Right Arm").transform.Find("Right Hand");
        item.transform.LookAt(transform.position - transform.forward * 10);
        item.transform.position = rightHand.position + Vector3.up * 0.1f;
        item.transform.parent = transform;
    }
}