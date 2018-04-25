using UnityEngine;

public class InventoryController : MonoBehaviour
{
    GameObject handleGO;
    RectTransform handleRectTransform;

    void Start()
    {
        handleGO = transform.Find("InventoryHandle").gameObject;
        handleRectTransform = handleGO.GetComponent<RectTransform>();
    }

    void Update()
    {
        //Vector3[] corners = new Vector3[4];
        //handleRectTransform.GetWorldCorners(corners);
        //Debug.Log("Handle: " + corners[0] + "," + corners[1] + "," + corners[2] + "," + corners[3]);
    }
}