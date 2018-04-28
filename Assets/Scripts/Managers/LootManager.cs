
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class LootManager
{
    static int itemNumber = 0;
    static Dictionary<int, InventoryItem> lootDictionary = new Dictionary<int, InventoryItem>();
    //static List<int> lootList = new List<int>();

    public static void Init()
    {
        //Here is where we generate all the base loot items in the game
        lootDictionary.Add(itemNumber++, new InventoryItem(itemNumber, "Hatchet", 0));
        lootDictionary.Add(itemNumber++, new InventoryItem(itemNumber, "Sword", 1));
        lootDictionary.Add(itemNumber++, new InventoryItem(itemNumber, "Shovel", 5));
        lootDictionary.Add(itemNumber++, new InventoryItem(itemNumber, "PickAxe", 10));
    }

    public static InventoryItem GetLoot()
    {
        //This method will become more sophisticated
        //with considerations for things like rarity, approx level, randomized stats, etc

        var selectedValues = lootDictionary.Values.Where(x => x.GetRequiredLevel() < 5).ToList();
        var rnd = Random.Range(0, selectedValues.Count());
        selectedValues[rnd].SetUniqueID(System.Guid.NewGuid());
        return selectedValues[rnd]; 
    }
}