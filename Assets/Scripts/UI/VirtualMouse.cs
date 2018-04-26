using UnityEngine;
using UnityEngine.UI;

public class VirtualMouse : MonoBehaviour
{
    float speed;
    bool isDragging;
    bool isLMBDown;
    bool isOverHandle;

    Vector3 motion;
    bool isAtHorizontalEdgeOfScreen;
    bool isAtVerticalEdgeOfScreen;
    float cursorWidthOffset;
    Vector3[] corners;
    RectTransform rectTransform;
    GameObject canvasGO;

    //this stuff hopefull gets replaced when i implement some kind of event system
    GameObject inventoryGO;
    GameObject handleGO;
    RectTransform handleRectTransform;
    

    void Start()
    {
        speed = 3500;
        isAtHorizontalEdgeOfScreen = false;
        isAtVerticalEdgeOfScreen = false;
        isDragging = false;
        isLMBDown = false;
        isOverHandle = false;
        corners = new Vector3[4];
        rectTransform = GetComponent<RectTransform>();
        canvasGO = transform.parent.gameObject;

        var image = GetComponent<Image>();
        cursorWidthOffset = image.rectTransform.rect.size.x * image.rectTransform.localScale.x / 2;

    }
    void Update()
    {
        //Movement
        gameObject.transform.SetAsLastSibling();
        motion = (Vector3.right * InputManager.mouseX) + (Vector3.up * InputManager.mouseY);
        gameObject.transform.position += motion * speed * Time.deltaTime;

        isAtHorizontalEdgeOfScreen = false;
        isAtVerticalEdgeOfScreen = false;
        KeepMouseOnScreen();



        //Object detection stuff
        rectTransform.GetWorldCorners(corners);

        Vector3[] handleCorners = new Vector3[4];
        inventoryGO = canvasGO.transform.Find("InventoryPanel(Clone)").gameObject;
        handleGO = inventoryGO.transform.Find("InventoryHandle").gameObject;
        handleRectTransform = handleGO.GetComponent<RectTransform>();
        handleRectTransform.GetWorldCorners(handleCorners);

     
        if (InputManager.isLMBDown)
        {
            isLMBDown = true;
        }
        if (InputManager.isLMBUp)
        {
            isLMBDown = false;
        }

        if (inventoryGO.activeSelf)
        {
            var rectX = handleCorners[0].x - 10;  // Dont know why I have to subtract 10 here
            var rectWidth = handleCorners[3].x - handleCorners[0].x;
            var rectHeight = handleCorners[1].y - handleCorners[0].y;
            var rectY = handleCorners[0].y - rectHeight;
            var handleRect = new Rect(rectX, rectY, rectWidth, rectHeight);

            if (!isLMBDown)
            {
                isOverHandle = handleRect.Contains(new Vector2(corners[0].x, corners[0].y));
            }

            if (isOverHandle)
            {
                if (isLMBDown)
                    isDragging = true;
                else
                    isDragging = false;
            }
        }

    }

    void LateUpdate()
    {
        if (isDragging)
        {
            var horizontalMotion = new Vector3(motion.x, 0, 0);
            var verticalMotion = new Vector3(0, motion.y, 0);
            var finalMotion = Vector3.zero;

            if (!isAtHorizontalEdgeOfScreen)
            {
                finalMotion += horizontalMotion;
            }
            if (!isAtVerticalEdgeOfScreen)
            {
                finalMotion += verticalMotion;
            }

            inventoryGO.transform.position += finalMotion * speed * Time.deltaTime;

        }
    }

    void KeepMouseOnScreen()
    {
        if (transform.position.x < cursorWidthOffset)
        {
            transform.position = new Vector3(cursorWidthOffset, transform.position.y, transform.position.z);
            isAtHorizontalEdgeOfScreen = true;
        }
        if (transform.position.x > Screen.width + cursorWidthOffset)
        {
            transform.position = new Vector3(Screen.width + cursorWidthOffset, transform.position.y, transform.position.z);
            isAtHorizontalEdgeOfScreen = true;
        }
        if (transform.position.y < -cursorWidthOffset)
        {
            transform.position = new Vector3(transform.position.x, -cursorWidthOffset, transform.position.z);
            isAtVerticalEdgeOfScreen = true;
        }
        if (transform.position.y > Screen.height - cursorWidthOffset)
        {
            transform.position = new Vector3(transform.position.x, Screen.height - cursorWidthOffset, transform.position.z);
            isAtVerticalEdgeOfScreen = true;
        }
    }


}
