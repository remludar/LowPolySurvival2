using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

#region old version with hardware mouse handler events
//public class InventoryHandleController : MonoBehaviour ,
//    IPointerEnterHandler,
//    IPointerExitHandler,
//    IDragHandler
//{

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//    }

//    public void OnDrag(PointerEventData eventData)
//    {
//        transform.parent.gameObject.transform.position += (Vector3)eventData.delta;
//    }


//}
#endregion

public class InventoryHandleController : MonoBehaviour
{
    void Update()
    {
        var rectTransform = GetComponent<RectTransform>();
        Debug.Log("ipHandle: " + rectTransform.localPosition);
    }

}
