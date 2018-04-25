using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum State { WANDERING, MELE_TARGET, RANGED, RANGED_TARGET };
    public State state = State.WANDERING;

    CharacterController cc;
    Vector3 motion;
    Quaternion lookRotation;
    public float moveSpeed;
    public float rotateSpeed;
    public float gravity = -9.8f;
    Vector3 lookDirection;

    InventoryItem currentlyWielding;
    Dictionary<string, InventoryItem> inventory;
    GameObject inventoryGO;

    #region O V E R L O A D S
    void Start()
    {
        cc = GetComponent<CharacterController>();
        inventory = new Dictionary<string, InventoryItem>();
        inventoryGO = Instantiate(Resources.Load("Prefabs/InventoryPanel")) as GameObject;
        inventoryGO.transform.SetParent(GameObject.Find("UI").transform.Find("Canvas"));
        inventoryGO.transform.localScale = new Vector3(2,2,2);
        inventoryGO.transform.localPosition = new Vector3(250, -125, 0);
        inventoryGO.SetActive(false);
        lookDirection = transform.forward;
    }
    void Update()
    {
        InputManager.Update();
        ProcessMovementRotationInput();
        ProcessAdditionalInput();
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // This awkward parent object check will go away when I import real models from blender
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
    #endregion

    #region H E L P E R S
    void ProcessMovementRotationInput()
    {
        var cameraForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
        var cameraRight = Camera.main.transform.right;
        motion = ((cameraRight * InputManager.horizontal) + (cameraForward * InputManager.vertical));

        // Fix diagonal speeds but preserve smoothing from GetAxis
        var mag = motion.magnitude;
        mag = Mathf.Clamp(mag, 0.0f, 1.0f);
        motion.Normalize();
        motion *= mag;
        motion = new Vector3(motion.x, motion.y + gravity, motion.z);
        motion *= moveSpeed * Time.deltaTime;

        // Rotation
        lookDirection += (InputManager.horizontalRaw * cameraRight) + (InputManager.verticalRaw * cameraForward);
        lookDirection = Vector3.ClampMagnitude(lookDirection, 1);
        if(lookDirection != Vector3.zero)
            lookRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        cc.Move(motion);
    }
    void ProcessAdditionalInput()
    {
        if (InputManager.isOne)
        {
            state = State.WANDERING;
        }

        if (InputManager.isTwo)
        {
            state = State.RANGED;
        }

        if (InputManager.isI)
        {
            inventoryGO.SetActive(!inventoryGO.activeSelf);
        }

        //if (InputManager.isLMBDown)
        //{
        //    currentlyWielding.Use();
        //}
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
    #endregion
}