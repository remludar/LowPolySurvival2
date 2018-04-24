using UnityEngine;

public class Sword : InventoryItem
{
    public override void Use()
    {
        Debug.Log("From Sword " + gameObject.name);
    }


}