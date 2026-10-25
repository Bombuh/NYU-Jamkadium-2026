using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenHUD : MonoBehaviour
{
    [Header("Order display: 3 icons for protein, veggie, sauce")]
    [SerializeField] private IngredientCatalog catalog;
    [SerializeField] private Image[] orderIcons;
    [SerializeField] private TMP_Text orderTimeText;
    [SerializeField] private TMP_Text roundTimeText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text cleanlinessText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text inventoryText;
    [SerializeField] private Inventory inventory;

    private void Start()
    {
        if (inventory != null)
        {
            inventory.Changed += RefreshInventory;
            RefreshInventory();
        }
    }
    private void OnDestroy()
    {
        if (inventory != null) inventory.Changed -= RefreshInventory;
    }

    public void SetRecipe(IReadOnlyList<string> ids)
    {
        for (int i = 0; i < orderIcons.Length; i++)
        {
            bool visible = i < ids.Count;
            orderIcons[i].gameObject.SetActive(visible);
            if (visible && catalog != null) orderIcons[i].sprite = catalog.GetSprite(ids[i]);
        }
    }
    public void RefreshOrder(CustomerOrder order)
    {
        if (orderTimeText != null)
            orderTimeText.text = "Order: " + Mathf.CeilToInt(Mathf.Max(0f, order.remainingSeconds)) + "s";
    }
    public void SetRoundTime(float seconds)
    {
        if (roundTimeText != null) roundTimeText.text = "Round: " + Mathf.CeilToInt(seconds) + "s";
    }
    public void SetRound(int round)
    {
        if (roundText != null) roundText.text = "Round " + round + "/3";
    }
    public void SetScore(int tips, int orders)
    {
        if (scoreText != null) scoreText.text = "Tips: $" + tips + " | Served: " + orders;
    }
    public void SetCleanliness(int value)
    {
        if (cleanlinessText != null) cleanlinessText.text = "Clean: " + value + "%";
    }
    public void SetMessage(string message)
    {
        if (messageText != null) messageText.text = message;
    }
    private void RefreshInventory()
    {
        if (inventoryText == null || inventory == null) return;
        inventoryText.text = "Bag: " + string.Join(", ", inventory.Ingredients);
    }
}
