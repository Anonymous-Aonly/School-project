using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FinalMapZoom : MonoBehaviour
{
    [Header("Drag the Empty GameObject (holding images) here")]
    public RectTransform mapContent;

    [Header("Objects that allow dragging/zooming (click to add)")]
    public List<GameObject> draggableObjects = new List<GameObject>();

    [Header("Settings")]
    public float zoomSpeed = 0.2f;
    public float minZoom = 0.5f;
    public float maxZoom = 5f;
    public float panSpeed = 1000f;

    private RectTransform panelRect;
    private GraphicRaycaster raycaster;

    public float currentZoom;
    private Vector2 targetPosition;
    private Vector2 dragOffset;

    private Vector2 baseMapSize;
    private bool isDragging;

    void Start()
    {
        panelRect = GetComponent<RectTransform>();
        raycaster = GetComponentInParent<GraphicRaycaster>();

        if (raycaster == null)
        {
            raycaster = gameObject.AddComponent<GraphicRaycaster>();
        }

        currentZoom = mapContent.localScale.x;
        targetPosition = mapContent.anchoredPosition;

        Vector3 oldScale = mapContent.localScale;
        mapContent.localScale = Vector3.one;

        LayoutRebuilder.ForceRebuildLayoutImmediate(mapContent);

        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(mapContent);
        baseMapSize = bounds.size;

        mapContent.localScale = oldScale;

        ClampMap();
        mapContent.anchoredPosition = targetPosition;
    }

    void Update()
    {
        if (mapContent == null)
            return;

        bool isOverPanel = RectTransformUtility.RectangleContainsScreenPoint(
            panelRect,
            Input.mousePosition,
            null);

        if (isOverPanel)
        {
            HandleZoom();
            HandlePan();
        }

        mapContent.localScale = Vector3.one * currentZoom;
        ClampMap();

        mapContent.anchoredPosition = Vector2.Lerp(
            mapContent.anchoredPosition,
            targetPosition,
            Time.deltaTime * 15f);
    }

    void HandleZoom()
    {
        // Don't zoom if hovering over non-draggable object
        if (!IsHoveringOnDraggableObject())
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) < 0.001f)
            return;

        currentZoom *= (1f + scroll * zoomSpeed);
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    void HandlePan()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"));

        targetPosition -= input * panSpeed * Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && IsClickingOnDraggableObject())
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panelRect,
                Input.mousePosition,
                null,
                out Vector2 mouse);

            dragOffset = targetPosition - mouse;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panelRect,
                Input.mousePosition,
                null,
                out Vector2 mouse);

            targetPosition = mouse + dragOffset;
        }
    }

    private bool IsHoveringOnDraggableObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if (results.Count == 0)
            return false;

        GameObject hitObject = results[0].gameObject;

        foreach (GameObject draggable in draggableObjects)
        {
            if (hitObject == draggable)
                return true;
        }

        return false;
    }

    private bool IsClickingOnDraggableObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        if (results.Count == 0)
            return false;

        GameObject hitObject = results[0].gameObject;

        foreach (GameObject draggable in draggableObjects)
        {
            if (hitObject == draggable)
                return true;
        }

        return false;
    }

    void ClampMap()
    {
        Vector2 scaledSize = baseMapSize * currentZoom;
        Vector2 panelSize = panelRect.rect.size;

        float limitX = Mathf.Max(0, (scaledSize.x - panelSize.x) / 2f);
        float limitY = Mathf.Max(0, (scaledSize.y - panelSize.y) / 2f);

        targetPosition.x = Mathf.Clamp(targetPosition.x, -limitX, limitX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -limitY, limitY);

        if (scaledSize.x <= panelSize.x)
            targetPosition.x = 0;

        if (scaledSize.y <= panelSize.y)
            targetPosition.y = 0;
    }
}