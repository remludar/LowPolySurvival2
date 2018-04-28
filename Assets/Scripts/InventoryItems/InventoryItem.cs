using UnityEngine;
using System;

public class InventoryItem 
{
    protected Guid uniqueID;
    protected int itemNumber;
    protected string itemName;
    protected int requiredLevel;

    public InventoryItem() { }

    public InventoryItem(int iNum, string iName, int reqLvl)
    {
        itemNumber = iNum;
        itemName = iName;
        requiredLevel = reqLvl;
    }

    public Guid GetUniqueID()
    {
        return uniqueID;
    }

    public string GetName()
    {
        return itemName;
    }

    public int GetRequiredLevel()
    {
        return requiredLevel;
    }

    public void SetUniqueID(Guid id)
    {
        uniqueID = id;
    }
}