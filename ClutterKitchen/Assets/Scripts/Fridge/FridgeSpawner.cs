
using System.Collections.Generic;
using UnityEngine;

public class FridgeSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private IngredientCatalog catalog;
    [SerializeField] private FridgeItem itemPrefab;
    [SerializeField] private RectTransform dragLayer;
    [SerializeField] private FridgeDropZone collectionZone;
    [SerializeField] private Inventory inventory;

    [Header("Spawn Zones")]
    [SerializeField] private RectTransform[] shelfZones;
    [SerializeField] private RectTransform[] doorZones;

    [Header("Clutter Settings")]
    [SerializeField] private int minimumExtras = 9;
    [SerializeField] private int maximumExtras = 16;
    [SerializeField] private float edgePadding = 4f;
    [SerializeField] private float verticalVariation = 5f;

    private readonly List<FridgeItem> spawned =
        new List<FridgeItem>();

    public void SetExtraCount(int min, int max)
    {
        minimumExtras = Mathf.Max(0, min);
        maximumExtras = Mathf.Max(minimumExtras, max);
    }

    public void GenerateForOrder(
        IReadOnlyList<string> finishedIds)
    {
        Clear();

        if (catalog == null || itemPrefab == null ||
            inventory == null || dragLayer == null)
        {
            Debug.LogError(
                "FridgeSpawner is missing required references!"
            );
            return;
        }

        if (finishedIds == null)
        {
            Debug.LogError("Order ingredient list is null!");
            return;
        }

        // Spawn every required ingredient first.
        foreach (string id in finishedIds)
        {
            string sourceId =
                IngredientCatalog.FridgeVersion(id);

            IngredientEntry entry = catalog.Find(sourceId);

            if (entry == null ||
                entry.sprite == null ||
                !entry.spawnInFridge)
            {
                Debug.LogError(
                    "Missing fridge ingredient: " + sourceId
                );
                continue;
            }

            Spawn(entry);
        }

        // Build a list of valid random clutter.
        List<IngredientEntry> choices =
            new List<IngredientEntry>();

        foreach (IngredientEntry entry in catalog.Items)
        {
            if (entry != null &&
                entry.spawnInFridge &&
                entry.sprite != null)
            {
                choices.Add(entry);
            }
        }

        if (choices.Count == 0)
            return;

        // Randomize the number of extra items.
        int min = Mathf.Max(0, minimumExtras);
        int max = Mathf.Max(min, maximumExtras);

        int extraCount = Random.Range(min, max + 1);

        for (int i = 0; i < extraCount; i++)
        {
            IngredientEntry randomIngredient =
                choices[Random.Range(0, choices.Count)];

            Spawn(randomIngredient);
        }

        Debug.Log(
            "Fridge generated with " +
            spawned.Count + " ingredients."
        );
    }

    private void Spawn(IngredientEntry entry)
    {
        // Choose the appropriate group of shelves.
        RectTransform[] zones =
            entry.doorItem ? doorZones : shelfZones;

        if (zones == null || zones.Length == 0)
        {
            Debug.LogWarning(
                "No shelf zones assigned for " + entry.id
            );
            return;
        }

        // Find a valid shelf.
        List<RectTransform> validZones =
            new List<RectTransform>();

        foreach (RectTransform zone in zones)
        {
            if (zone != null)
                validZones.Add(zone);
        }

        if (validZones.Count == 0)
            return;

        // Pick a random shelf.
        RectTransform selectedZone =
            validZones[Random.Range(0, validZones.Count)];

        // Spawn ingredient as a child of that shelf.
        FridgeItem item =
            Instantiate(itemPrefab, selectedZone);

        RectTransform itemRect =
            item.GetComponent<RectTransform>();

        // Center horizontally, bottom pivot vertically.
        itemRect.anchorMin = new Vector2(0.5f, 0.5f);
        itemRect.anchorMax = new Vector2(0.5f, 0.5f);
        itemRect.pivot = new Vector2(0.5f, 0f);

        // Keep sprite entirely inside shelf width.
        float halfWidth = itemRect.rect.width * 0.5f;

        float minX =
            -selectedZone.rect.width * selectedZone.pivot.x
            + halfWidth + edgePadding;

        float maxX =
            selectedZone.rect.width *
            (1f - selectedZone.pivot.x)
            - halfWidth - edgePadding;

        float x = (minX <= maxX)
            ? Random.Range(minX, maxX)
            : selectedZone.rect.center.x;

        // Place the item's bottom near the shelf surface.
        float shelfBottom =
            -selectedZone.rect.height *
            selectedZone.pivot.y;

        float y = shelfBottom +
                  Random.Range(0f, Mathf.Max(0f, verticalVariation));

        itemRect.anchoredPosition =
            new Vector2(x, y);

        // Assign sprite and inventory behavior.
        item.Setup(
            entry.id,
            entry.sprite,
            inventory,
            collectionZone,
            dragLayer
        );

        spawned.Add(item);
    }

    private void Clear()
    {
        foreach (FridgeItem item in spawned)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        spawned.Clear();
    }
    [ContextMenu("Generate Test Fridge")]
    private void GenerateTestFridge()
    {
        GenerateForOrder(new string[]
        {
        "beef_cooked",
        "lettuce_slice",
        "ketchup"
        });
    }
}
