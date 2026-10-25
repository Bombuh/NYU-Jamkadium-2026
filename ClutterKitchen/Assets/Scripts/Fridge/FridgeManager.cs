
using UnityEngine;

public class FridgeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject fridgePanel;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInteraction playerInteraction;

    public bool IsOpen { get; private set; }

    private void Start()
    {
        fridgePanel.SetActive(false);
        IsOpen = false;
    }

    public void OpenFridge()
    {
        if (IsOpen) return;

        IsOpen = true;
        fridgePanel.SetActive(true);

        playerController.enabled = false;
        playerInteraction.enabled = false;
    }

    public void CloseFridge()
    {
        if (!IsOpen) return;

        IsOpen = false;
        fridgePanel.SetActive(false);

        playerController.enabled = true;
        playerInteraction.enabled = true;
    }
}
