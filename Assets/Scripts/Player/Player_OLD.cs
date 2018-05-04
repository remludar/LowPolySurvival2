using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_OLD : MonoBehaviour
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
    Dictionary<Guid, InventoryItem> inventory;
    GameObject inventoryGO;

    GameObject mouseGO;


    #region O V E R L O A D S
    void Start()
    {
        cc = GetComponent<CharacterController>();
        lookDirection = transform.forward;

        inventory = new Dictionary<Guid, InventoryItem>();
        inventoryGO = Instantiate(Resources.Load("Prefabs/InventoryPanel")) as GameObject;
        inventoryGO.transform.SetParent(GameObject.Find("UI").transform.Find("Canvas"));
        inventoryGO.transform.localScale = new Vector3(2,2,2);
        inventoryGO.transform.localPosition = new Vector3(662, -227, 0);
        inventoryGO.SetActive(false);

        mouseGO = GameObject.Find("UI").transform.Find("Canvas").transform.Find("Mouse").gameObject;

    }
    void Update()
    {
        InputManager.Update();
        ProcessMovementRotationInput();
        ProcessAdditionalInput();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Loot")
        {
            Destroy(collision.gameObject);
            var loot = LootManager.GetLoot();

            //Somehow add this to my inventory and show it in the inventory UI in game
            AddToInventory(loot);
            
            Debug.Log("You found a " + loot.GetName() + "!");
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

        if (InputManager.isRMB)
        {
            mouseGO.SetActive(false);
        }
        else
        {
            mouseGO.SetActive(true);
        }



        //if (InputManager.isRMB)
        //{
        //    Cursor.visible = false;
        //    Cursor.lockState = CursorLockMode.Locked;
        //}
        //else
        //{
        //    Cursor.visible = true;
        //    Cursor.lockState = CursorLockMode.None;
        //}

        //if (InputManager.isLMBDown)
        //{
        //    currentlyWielding.Use();
        //}
    }
    void AddToInventory(InventoryItem item)
    {
        //really should have a static inventory manager or something to let me do this.
        var inventoryGO = GameObject.Find("UI").transform.Find("Canvas").transform.Find("InventoryPanel(Clone)").transform.Find("Inventory").gameObject;
        var totalSpaces = inventoryGO.transform.childCount;
        var nextEmptySpace = "InventorySlot" + inventory.Count;
        Debug.Log(nextEmptySpace);

        var slot0 = inventoryGO.transform.Find(nextEmptySpace).gameObject;
        var color = slot0.GetComponent<Image>().color;
        slot0.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 1.0f);
        var path = "Sprites/" + item.GetName() + "Icon";
        slot0.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>(path);

        inventory.Add(item.GetUniqueID(), item);


    }
    void Wield(InventoryItem item)
    {
        currentlyWielding = item;
        var rightHand = GameObject.Find("Right Arm").transform.Find("Right Hand");
        //item.transform.LookAt(transform.position - transform.forward * 10);
        //item.transform.position = rightHand.position + Vector3.up * 0.1f;
        //item.transform.parent = transform;
    }
    #endregion
}