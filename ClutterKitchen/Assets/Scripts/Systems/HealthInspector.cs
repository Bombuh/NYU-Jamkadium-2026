using UnityEngine;

public class HealthInspector : MonoBehaviour
{
    [SerializeField] private CleanlinessManager cleanliness;
    [Range(0,100)] [SerializeField] private int passingScore = 60;

    public bool Inspect()
    {
        if (cleanliness == null) return true;
        bool passed = cleanliness.Cleanliness >= passingScore;
        Debug.Log(passed ? "Inspection passed!" : "Kitchen closed: inspection failed!");
        return passed;
    }
}
