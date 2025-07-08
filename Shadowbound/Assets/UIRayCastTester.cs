using UnityEngine;
using UnityEngine.EventSystems;

public class UIRayCastTester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log($"UI hits: {results.Count}");
            foreach (var hit in results)
            {
                Debug.Log($"UI hit: {hit.gameObject.name}");
            }
        }
    }
}
