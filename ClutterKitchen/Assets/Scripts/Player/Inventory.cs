using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity = 12;
    [SerializeField] private List<string> ingredients = new List<string>();
    public IReadOnlyList<string> Ingredients => ingredients;
    public event Action Changed;

    public bool AddIngredient(string id)
    {
        if (ingredients.Count >= capacity) return false;
        ingredients.Add(id);
        Changed?.Invoke();
        Debug.Log("Collected " + id);
        return true;
    }

    public bool Has(string id) => ingredients.Contains(id);

    public bool Remove(string id)
    {
        bool removed = ingredients.Remove(id);
        if (removed) Changed?.Invoke();
        return removed;
    }

    public int TransformAll(Func<string, string> transform)
    {
        int changed = 0;
        for (int i = 0; i < ingredients.Count; i++)
        {
            string next = transform(ingredients[i]);
            if (next == ingredients[i]) continue;
            ingredients[i] = next;
            changed++;
        }
        if (changed > 0) Changed?.Invoke();
        return changed;
    }

    public void Clear()
    {
        ingredients.Clear();
        Changed?.Invoke();
    }
}
