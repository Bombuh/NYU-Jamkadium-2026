using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class FridgeDropZone : MonoBehaviour
{
    private RectTransform rect;
    private void Awake() => rect = GetComponent<RectTransform>();
    public bool Contains(PointerEventData eventData)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            rect, eventData.position, eventData.pressEventCamera);
    }
}
