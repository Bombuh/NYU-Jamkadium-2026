using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private KitchenHUD hud;
    public int TotalTips { get; private set; }
    public int CompletedOrders { get; private set; }

    private void Start() => hud?.SetScore(TotalTips, CompletedOrders);

    public void AddTip(int dollars)
    {
        TotalTips += dollars;
        CompletedOrders++;
        hud?.SetScore(TotalTips, CompletedOrders);
    }
}
