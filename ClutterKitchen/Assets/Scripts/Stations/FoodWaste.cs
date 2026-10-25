using UnityEngine;

// Optional: place on waste prefab with a Collider2D and Interactable layer.
public class FoodWaste : MonoBehaviour, IInteractable
{
    [SerializeField] private CleanlinessManager cleanliness;
    [SerializeField] private int cleaningPoints = 5;

    public void Interact(PlayerInteraction player)
    {
        if (cleanliness == null)
            cleanliness = FindFirstObjectByType<CleanlinessManager>();
        cleanliness?.Clean(cleaningPoints);
        Destroy(gameObject);
    }
}
