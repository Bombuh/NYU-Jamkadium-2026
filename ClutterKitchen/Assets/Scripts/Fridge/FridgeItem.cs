
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

    // Prevent multiple items from being dragged at once.
    private static FridgeItem activeDraggedItem;

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
        if (activeDraggedItem != null)
            return;

        if (EventSystem.current == null || dragLayer == null)
            return;

        PointerEventData pointer =
            new PointerEventData(EventSystem.current);

        pointer.position = Input.mousePosition;

        List<RaycastResult> hits = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointer, hits);

        // Find the first ingredient detected under the cursor.
        FridgeItem clickedItem = null;

        foreach (RaycastResult hit in hits)
        {
            FridgeItem found =
                hit.gameObject.GetComponentInParent<FridgeItem>();

            if (found != null)
            {
                clickedItem = found;
                break;
            }
        }

        // Only the selected ingredient can start dragging.
        if (clickedItem != this)
            return;

        isDragging = true;
        activeDraggedItem = this;
        originalParent = transform.parent;

        // Move item above other UI elements during dragging.
        rect.SetParent(dragLayer, true);
        rect.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;

        FollowMouse();

        Debug.Log("GRABBED: " + ingredientID);
    }

    private void FollowMouse()
    {
        if (dragLayer == null)
            return;

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
        activeDraggedItem = null;

        canvasGroup.blocksRaycasts = true;

        if (EventSystem.current == null)
            return;

        PointerEventData pointer =
            new PointerEventData(EventSystem.current);

        pointer.position = Input.mousePosition;

        // Check whether item was dropped into collection basket.
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

        // Otherwise, leave the food where it was dropped.
        Vector3 droppedPosition = rect.position;

        rect.SetParent(originalParent, true);
        rect.position = droppedPosition;
        rect.SetAsLastSibling();
    }

    private void OnDisable()
    {
        if (activeDraggedItem == this)
        {
            activeDraggedItem = null;
        }

        isDragging = false;
    }
}

