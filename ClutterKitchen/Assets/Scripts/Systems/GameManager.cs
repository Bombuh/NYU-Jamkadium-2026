using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private OrderManager orders;
    [SerializeField] private FridgeSpawner fridgeSpawner;
    [SerializeField] private HealthInspector inspector;
    [SerializeField] private KitchenHUD hud;
    [SerializeField] private float roundLengthSeconds = 180f;
    [SerializeField] private int totalRounds = 3;

    public int Round { get; private set; } = 1;
    private float remaining;
    private bool gameOver;

    private void Start()
    {
        remaining = roundLengthSeconds;
        ApplyDifficulty();
        hud?.SetRound(Round);
    }

    private void Update()
    {
        if (gameOver) return;
        remaining -= Time.deltaTime;
        hud?.SetRoundTime(Mathf.Max(0f, remaining));
        if (remaining <= 0f) EndRound();
    }

    private void ApplyDifficulty()
    {
        if (Round == 1) { fridgeSpawner?.SetExtraCount(8,12); orders?.SetPatienceMultiplier(1f); }
        else if (Round == 2) { fridgeSpawner?.SetExtraCount(12,17); orders?.SetPatienceMultiplier(.85f); }
        else { fridgeSpawner?.SetExtraCount(17,22); orders?.SetPatienceMultiplier(.7f); }
    }

    private void EndRound()
    {
        orders?.SetRunning(false);
        if (inspector != null && !inspector.Inspect())
        {
            Finish("Kitchen CLOSED: failed health inspection!");
            return;
        }
        if (Round >= totalRounds)
        {
            Finish("You survived all rounds! Kitchen saved!");
            return;
        }
        Round++;
        remaining = roundLengthSeconds;
        ApplyDifficulty();
        hud?.SetRound(Round);
        orders?.NewOrder();
    }

    private void Finish(string message)
    {
        gameOver = true;
        orders?.SetRunning(false);
        hud?.SetMessage(message);
        Debug.Log(message);
        Time.timeScale = 0f;
    }
}
