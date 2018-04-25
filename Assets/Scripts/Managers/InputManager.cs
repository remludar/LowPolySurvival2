using UnityEngine;

public static class InputManager
{
    public static float horizontal = 0.0f;
    public static float vertical = 0.0f;
    public static float horizontalRaw = 0.0f;
    public static float verticalRaw = 0.0f;
    public static float mouseX = 0.0f;
    public static float mouseY = 0.0f;

    public static bool isLMBDown = false;
    public static bool isLMB = false;
    public static bool isLMBUp = false;
    public static bool isRMB = false;
    public static bool isSpace = false;
    public static bool isEsc = false;
    public static bool isOne = false;
    public static bool isTwo = false;
    public static bool isThree = false;
    public static bool isLeftShift = false;
    public static bool isLeftAlt = false;
    public static bool isLeftCtrl = false;
    public static bool isI = false;

    public static void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        horizontalRaw = Input.GetAxisRaw("Horizontal");
        verticalRaw = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");


        isLMBDown = Input.GetMouseButtonDown(0);
        isLMB = Input.GetMouseButton(0);
        isLMBUp = Input.GetMouseButtonUp(0);
        isRMB = Input.GetMouseButton(1);
        isSpace = Input.GetKeyDown(KeyCode.Space);
        isEsc = Input.GetKeyDown(KeyCode.Escape);
        isOne = Input.GetKeyDown(KeyCode.Alpha1);
        isTwo = Input.GetKeyDown(KeyCode.Alpha2);
        isThree = Input.GetKeyDown(KeyCode.Alpha3);
        isLeftShift = Input.GetKey(KeyCode.LeftShift);
        isLeftAlt = Input.GetKey(KeyCode.LeftAlt);
        isLeftCtrl = Input.GetKey(KeyCode.LeftControl);
        isI = Input.GetKeyDown(KeyCode.I);
    }
}