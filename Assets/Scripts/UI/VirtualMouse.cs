using UnityEngine;

public class VirtualMouse : MonoBehaviour
{
    public float speed;

    GameObject inventoryGO;
    GameObject handleGO;
    RectTransform handleRectTransform;

    bool isDragging = false;
    Vector3 motion;

    void Start()
    {
        

    }
    void Update()
    {
        gameObject.transform.SetAsLastSibling();
        motion = (Vector3.right * InputManager.mouseX) + (Vector3.up * InputManager.mouseY);
        gameObject.transform.position += motion * speed * Time.deltaTime;

        var rectTransform = GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        var canvasGO = transform.parent.gameObject;
        Vector3[] handleCorners = new Vector3[4];
        if (!isDragging)
        {
            inventoryGO = canvasGO.transform.Find("InventoryPanel(Clone)").gameObject;
            handleGO = inventoryGO.transform.Find("InventoryHandle").gameObject;
            handleRectTransform = handleGO.GetComponent<RectTransform>();

            
            handleRectTransform.GetWorldCorners(handleCorners);
        }

        var rectX = handleCorners[0].x - 10;  // Dont know why I have to subtract 10 here
        var rectWidth = handleCorners[3].x - handleCorners[0].x;
        var rectHeight = handleCorners[1].y - handleCorners[0].y;
        var rectY = handleCorners[0].y - rectHeight;
        var handleRect = new Rect(rectX, rectY, rectWidth, rectHeight);

        if (Input.GetMouseButton(0))
        {
            if (handleRect.Contains(new Vector2(corners[0].x, corners[0].y)))
            {
                isDragging = true;
            }
        }
        else
        {
            isDragging = false;
        }
    }

    void LateUpdate()
    {
        if (isDragging)
            inventoryGO.transform.position += motion * speed * Time.deltaTime;
    }


}
