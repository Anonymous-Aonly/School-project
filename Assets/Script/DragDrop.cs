using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;

    [Header("Identification")]
    public string CountryName;

    [Header("Sprites")]
    public Sprite PinImage;
    public Sprite FlagImage;

    [Header("Drop Settings")]
    public float snapDistance = 100f;

    [Header("Positioning")]
    [Tooltip("Offset from the center of the slot when dropped")]
    public Vector2 slotOffset = new Vector2(-150f, -30f);

    [Header("Animation Settings")]
    public float squishDuration = 0.3f;
    public float squishAmount = 0.3f;
    public int bounceCount = 2;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalAnchoredPosition;
    public bool IsValid;
    private Image imageComponent;
    private Vector2 OriginalScale;

    private ItemSlot lastHighlightedSlot;
    private bool isDragging;

    private void Awake()
    {
        imageComponent = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalParent = transform.parent;
        OriginalScale = rectTransform.localScale;
        originalAnchoredPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        
        // Check the CURRENT parent to clear the slot properly
        if (transform.parent != null)
        {
            ItemSlot currentSlot = transform.parent.GetComponent<ItemSlot>();
            if (currentSlot != null)
            {
                currentSlot.RemoveFlag();
            }
        }

        transform.SetParent(canvas.transform);
        rectTransform.localScale = OriginalScale * 1.1f; 
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
        imageComponent.sprite = PinImage;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        HandleSlotHighlighting();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true; 

        if (lastHighlightedSlot != null)
        {
            lastHighlightedSlot.ResetColor();
            lastHighlightedSlot = null;
        }

        ItemSlot targetSlot = FindClosestSlot();

        if (targetSlot != null)
        {
            SnapToSlot(targetSlot);
        }
        else
        {
            ReturnToOriginal();
        }
    }

    private void SnapToSlot(ItemSlot slot)
    {
        transform.SetParent(slot.transform);
        rectTransform.anchoredPosition = slotOffset; 
        
        // Set scale to 1,1. The parent ItemSlot handles all zoom scaling automatically!
        rectTransform.localScale = Vector2.one;
        
        IsValid = slot.CountryName == CountryName;
        imageComponent.sprite = FlagImage;

        slot.OnFlagDropped();

        // Squish around Vector2.one
        StartCoroutine(SquishAnimation(Vector2.one));
    }

    private void ReturnToOriginal()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalAnchoredPosition;
        rectTransform.localScale = OriginalScale;
        IsValid = false;
        imageComponent.sprite = FlagImage;
    }

    private IEnumerator SquishAnimation(Vector2 targetScale)
    {
        float elapsed = 0f;
        float totalDuration = squishDuration * (bounceCount + 1);

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / totalDuration;

            float wave = Mathf.Sin(t * Mathf.PI * bounceCount) * Mathf.Exp(-t * 4f);

            float scaleX = targetScale.x * (1f + wave * squishAmount);
            float scaleY = targetScale.y * (1f - wave * squishAmount);

            rectTransform.localScale = new Vector2(scaleX, scaleY);

            yield return null;
        }

        // Ensure exact target scale at end
        rectTransform.localScale = targetScale;
    }

    private void HandleSlotHighlighting()
    {
        ItemSlot closestSlot = FindClosestSlot();

        if (closestSlot != lastHighlightedSlot)
        {
            if (lastHighlightedSlot != null)
                lastHighlightedSlot.ResetColor();

            lastHighlightedSlot = closestSlot;

            if (closestSlot != null && !closestSlot.HasAnyFlag())
            {
                closestSlot.Highlight();
            }
        }
    }

    private ItemSlot FindClosestSlot()
    {
        ItemSlot[] allSlots = FindObjectsOfType<ItemSlot>();
        ItemSlot closest = null;
        float closestDist = snapDistance;

        foreach (ItemSlot slot in allSlots)
        {
            if (slot.HasAnyFlag()) continue;

            float distance = Vector2.Distance(transform.position, slot.transform.position);

            if (distance < closestDist)
            {
                closestDist = distance;
                closest = slot;
            }
        }

        return closest;
    }

    public void OnPointerDown(PointerEventData eventData) { }
}