using UnityEngine;

public class FridgeInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private FridgeManager fridgeManager;

    public void Interact(PlayerInteraction player)
    {
        fridgeManager.OpenFridge();
    }
}
