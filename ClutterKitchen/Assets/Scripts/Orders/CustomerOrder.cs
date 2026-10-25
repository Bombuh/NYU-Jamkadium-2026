using System.Collections.Generic;

[System.Serializable]
public class CustomerOrder
{
    public List<string> required = new List<string>();
    public float totalSeconds;
    public float remainingSeconds;

    public CustomerOrder(List<string> ingredientIds, float seconds)
    {
        required = ingredientIds;
        totalSeconds = seconds;
        remainingSeconds = seconds;
    }
}
