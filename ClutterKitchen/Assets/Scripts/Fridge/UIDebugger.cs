
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDebugger : MonoBehaviour
{
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (EventSystem.current == null)
        {
            Debug.LogError("NO EVENT SYSTEM!");
            return;
        }

        PointerEventData pointer = new PointerEventData(
            EventSystem.current
        );

        pointer.position = Input.mousePosition;

        List<RaycastResult> results =
            new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointer, results);

        if (results.Count == 0)
        {
            Debug.Log("Mouse hit NO UI objects.");
            return;
        }

        foreach (RaycastResult result in results)
        {
            Debug.Log("Mouse hit: " + result.gameObject.name);
        }
    }
}
