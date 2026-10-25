using UnityEngine;

public enum StationAction { Chop, Cook, Assemble, Serve, Clean, DiscardBag }

// Put on the visual parent of your existing click-collider child.
// Keep FridgeInteractable separate; do not put this on the fridge.
public class KitchenStation : MonoBehaviour, IInteractable
{
    [SerializeField] private StationAction action;
    [SerializeField] private Inventory inventory;
    [SerializeField] private OrderManager orders;
    [SerializeField] private CleanlinessManager cleanliness;
    [SerializeField] private KitchenHUD hud;
    [SerializeField] private GameObject wastePrefab;
    [SerializeField] private Transform wasteSpawnPoint;

    public void Interact(PlayerInteraction player)
    {
        switch (action)
        {
            case StationAction.Chop:
                if (inventory == null) return;
                int chopped = inventory.TransformAll(IngredientCatalog.ChopVersion);
                if (chopped > 0)
                {
                    cleanliness?.AddMess(5 * chopped);
                    SpawnWaste(chopped);
                    hud?.SetMessage("Chopped " + chopped + " vegetables.");
                }
                else hud?.SetMessage("No whole vegetables in your bag.");
                break;
            case StationAction.Cook:
                if (inventory == null) return;
                int cooked = inventory.TransformAll(IngredientCatalog.CookVersion);
                if (cooked > 0)
                {
                    cleanliness?.AddMess(3 * cooked);
                    hud?.SetMessage("Cooked " + cooked + " proteins.");
                }
                else hud?.SetMessage("No raw proteins in your bag.");
                break;
            case StationAction.Assemble:
                orders?.TryAssemble();
                break;
            case StationAction.Serve:
                orders?.TryDeliver();
                break;
            case StationAction.Clean:
                cleanliness?.Clean(15);
                hud?.SetMessage("Station cleaned!");
                break;
            case StationAction.DiscardBag:
                inventory?.Clear();
                hud?.SetMessage("Bag emptied.");
                break;
        }
    }

    private void SpawnWaste(int count)
    {
        if (wastePrefab == null) return;
        Transform spot = wasteSpawnPoint == null ? transform : wasteSpawnPoint;
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = (Vector3)Random.insideUnitCircle * 0.3f;
            Instantiate(wastePrefab, spot.position + offset, Quaternion.identity);
        }
    }
}
