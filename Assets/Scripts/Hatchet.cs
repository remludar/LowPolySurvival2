using UnityEngine;

public class Hatchet : InventoryItem
{
    public override void Use()
    {
        Debug.Log("From Hatchet " + gameObject.name);
    }

    
}