using UnityEngine;

public class InventoryController : MonoBehaviour
{
    GameObject handleGO;
    RectTransform handleRectTransform;
    GameObject mouseGO;

    void Start()
    {
        handleGO = transform.Find("InventoryHandle").gameObject;
        handleRectTransform = handleGO.GetComponent<RectTransform>();
        mouseGO = GameObject.FindGameObjectWithTag("Mouse");
    }

   
}