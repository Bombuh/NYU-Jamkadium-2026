using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class FridgeItem : MonoBehaviour
{
    private string ingredientID;
    private Inventory inventory;
    private FridgeDropZone collectionZone;
    private RectTransform dragLayer;

    private RectTransform rect;
    private CanvasGroup canvasGroup;

    private Transform originalParent;
    private bool isDragging;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(
        string id,
        Sprite sprite,
        Inventory playerInventory,
        FridgeDropZone dropZone,
        RectTransform canvasLayer)
    {
        ingredientID = id;
        inventory = playerInventory;
        collectionZone = dropZone;
        dragLayer = canvasLayer;

        Image image = GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = true;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            TryGrab();
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            FollowMouse();
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            ReleaseItem();
        }
    }

    private void TryGrab()
    {
        if (EventSystem.current == null || dragLayer == null)
            return;

        PointerEventData pointer =
            new PointerEventData(EventSystem.current);

        pointer.position = Input.mousePosition;

        List<RaycastResult> hits = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointer, hits);

        // Only grab the topmost UI object.
        if (hits.Count == 0 || hits[0].gameObject != gameObject)
            return;

        isDragging = true;
        originalParent = transform.parent;

        rect.SetParent(dragLayer, true);
        rect.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;

        FollowMouse();

        Debug.Log("GRABBED: " + ingredientID);
    }

    private void FollowMouse()
    {
        Canvas canvas = dragLayer.GetComponentInParent<Canvas>();

        Camera uiCamera = null;

        if (canvas != null &&
            canvas.rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.rootCanvas.worldCamera;
        }

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            dragLayer,
            Input.mousePosition,
            uiCamera,
            out Vector3 worldPosition))
        {
            rect.position = worldPosition;
        }
    }

    private void ReleaseItem()
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        PointerEventData pointer =
            new PointerEventData(EventSystem.current);

        pointer.position = Input.mousePosition;

        if (collectionZone != null &&
            collectionZone.Contains(pointer))
        {
            if (inventory != null &&
                inventory.AddIngredient(ingredientID))
            {
                Debug.Log("COLLECTED: " + ingredientID);
                Destroy(gameObject);
                return;
            }
        }

        // Leave the item where it was dropped.
        Vector3 droppedPosition = rect.position;

        rect.SetParent(originalParent, true);
        rect.position = droppedPosition;
        rect.SetAsLastSibling();
    }
}
