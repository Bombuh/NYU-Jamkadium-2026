using UnityEngine;

public class CleanlinessManager : MonoBehaviour
{
    [SerializeField] private KitchenHUD hud;
    [Range(0,100)] [SerializeField] private int cleanliness = 100;
    public int Cleanliness => cleanliness;

    private void Start() => hud?.SetCleanliness(cleanliness);

    public void AddMess(int amount)
    {
        cleanliness = Mathf.Clamp(cleanliness - Mathf.Abs(amount), 0, 100);
        hud?.SetCleanliness(cleanliness);
    }

    public void Clean(int amount)
    {
        cleanliness = Mathf.Clamp(cleanliness + Mathf.Abs(amount), 0, 100);
        hud?.SetCleanliness(cleanliness);
    }
}
