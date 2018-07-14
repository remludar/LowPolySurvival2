using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController cc;
    Vector3 motion;

    float moveSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float gravity = -9.8f;
    public float rotXSpeed = 5f;
    public float rotYSpeed = 5f;
    float rotationY = 0.0f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        GetComponent<MeshRenderer>().enabled = false;
    }

    void Update()
    {
        InputManager.Update();
        ProcessMovementRotationInput();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void ProcessMovementRotationInput()
    {
        motion = (InputManager.horizontal * transform.right) + (InputManager.vertical * transform.forward);

        if (InputManager.isE)
            motion += Vector3.up;

        if (InputManager.isQ)
            motion += Vector3.down;

        if (InputManager.isLeftShift)
            moveSpeed = runSpeed;
        else
        {
            moveSpeed = walkSpeed;
        }

        // Fix diagonal speeds but preserve smoothing from GetAxis
        var mag = motion.magnitude;
        mag = Mathf.Clamp(mag, 0.0f, 1.0f);
        motion.Normalize();
        motion *= mag;
        motion = new Vector3(motion.x, motion.y + gravity, motion.z);
        motion *= moveSpeed * Time.deltaTime;

        var rotationX = transform.localEulerAngles.y + InputManager.mouseX * rotXSpeed;

        rotationY += InputManager.mouseY * rotYSpeed;
        rotationY = Mathf.Clamp(rotationY, -90.0f, 90.0f);

        //rotate body horizontally only
        transform.localEulerAngles = new Vector3(0, rotationX, 0);

        //rotate arms vertically only
        transform.Find("Arms").localEulerAngles = new Vector3(-rotationY, 0, 0);

        cc.Move(motion);

    }
}