using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private FridgeSpawner fridgeSpawner;
    [SerializeField] private Inventory inventory;
    [SerializeField] private KitchenHUD hud;
    [SerializeField] private ScoreManager score;
    [SerializeField] private float startingOrderSeconds = 85f;

    [Header("Recipe Pools (finished ingredient IDs)")]
    [SerializeField] private List<string> proteins = new List<string>
        { "beef_cooked", "turkey_cooked", "chicken_cooked" };
    [SerializeField] private List<string> vegetables = new List<string>
        { "lettuce_slice", "tomato_slice", "onion_slice", "pickle_slice", "avocado_slice" };
    [SerializeField] private List<string> sauces = new List<string>
        { "ketchup", "mayo", "mustard", "hot_sauce", "bbq_sauce", "house_sauce" };

    public CustomerOrder CurrentOrder { get; private set; }
    public bool OrderAssembled { get; private set; }
    private bool running;
    private float patienceMultiplier = 1f;

    private void Start() { NewOrder(); }

    private void Update()
    {
        if (!running || CurrentOrder == null) return;
        CurrentOrder.remainingSeconds -= Time.deltaTime;
        hud?.RefreshOrder(CurrentOrder);
        if (CurrentOrder.remainingSeconds <= 0f)
        {
            Debug.Log("Order expired!");
            NewOrder();
        }
    }

    public void SetPatienceMultiplier(float multiplier) => patienceMultiplier = Mathf.Max(.2f, multiplier);

    public void SetRunning(bool value) => running = value;

    public void NewOrder()
    {
        if (inventory != null) inventory.Clear();
        OrderAssembled = false;
        List<string> recipe = new List<string>();
        if (proteins.Count == 0 || vegetables.Count == 0 || sauces.Count == 0)
        {
            Debug.LogError("Order recipe pools must not be empty.");
            return;
        }
        recipe.Add(proteins[Random.Range(0, proteins.Count)]);
        recipe.Add(vegetables[Random.Range(0, vegetables.Count)]);
        recipe.Add(sauces[Random.Range(0, sauces.Count)]);
        CurrentOrder = new CustomerOrder(recipe, startingOrderSeconds * patienceMultiplier);
        fridgeSpawner?.GenerateForOrder(CurrentOrder.required);
        hud?.SetRecipe(CurrentOrder.required);
        hud?.RefreshOrder(CurrentOrder);
        running = true;
    }

    public bool TryAssemble()
    {
        if (!running || CurrentOrder == null || inventory == null) return false;
        if (OrderAssembled) return true;
        // Exact match, including quantities; extra/wrong ingredients are not allowed.
        List<string> unused = new List<string>(inventory.Ingredients);
        foreach (string required in CurrentOrder.required)
        {
            if (!unused.Remove(required))
            {
                hud?.SetMessage("Missing or unprepared ingredients!");
                return false;
            }
        }
        if (unused.Count != 0)
        {
            hud?.SetMessage("Extra ingredients! Discard them first.");
            return false;
        }
        OrderAssembled = true;
        hud?.SetMessage("Sandwich assembled! Serve at the window.");
        return true;
    }

    public void TryDeliver()
    {
        if (!OrderAssembled)
        {
            hud?.SetMessage("Assemble at the counter first!");
            return;
        }
        float fraction = Mathf.Clamp01(CurrentOrder.remainingSeconds / CurrentOrder.totalSeconds);
        int tip = Mathf.Clamp(Mathf.CeilToInt(fraction * 5f), 1, 5);
        score?.AddTip(tip);
        hud?.SetMessage("Order delivered! +$" + tip);
        NewOrder();
    }
}
