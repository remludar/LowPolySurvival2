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
    }

    void Update()
    {
        InputManager.Update();
        ProcessMovementRotationInput();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void ProcessMovementRotationInput()
    {
        motion = (InputManager.horizontal * transform.right) + (InputManager.vertical * transform.forward) ;

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
        //transform.localEulerAngles = new Vector3(0, rotationX, 0);

        rotationY += InputManager.mouseY * rotYSpeed;
        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

        cc.Move(motion);

    }
}