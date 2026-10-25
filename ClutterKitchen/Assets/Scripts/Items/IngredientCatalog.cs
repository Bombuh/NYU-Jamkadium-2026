using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IngredientEntry
{
    public string id;
    public Sprite sprite;
    public bool doorItem;
    public bool spawnInFridge = true;
}

// Single catalog: add your art sprites once in this component's Inspector.
public class IngredientCatalog : MonoBehaviour
{
    [SerializeField] private List<IngredientEntry> items = new List<IngredientEntry>();
    public IReadOnlyList<IngredientEntry> Items => items;

    public IngredientEntry Find(string id)
    {
        return items.Find(x => x.id == id);
    }

    public Sprite GetSprite(string id)
    {
        IngredientEntry item = Find(id);
        return item == null ? null : item.sprite;
    }

    // Orders use prepared IDs, but the fridge supplies raw/whole versions.
    public static string FridgeVersion(string preparedId)
    {
        if (preparedId.EndsWith("_cooked"))
            return preparedId.Replace("_cooked", "_raw");
        if (preparedId.EndsWith("_slice"))
            return preparedId.Replace("_slice", "_whole");
        return preparedId;
    }

    public static string CookVersion(string id)
    {
        return id.EndsWith("_raw") ? id.Replace("_raw", "_cooked") : id;
    }

    public static string ChopVersion(string id)
    {
        return id.EndsWith("_whole") ? id.Replace("_whole", "_slice") : id;
    }
}
