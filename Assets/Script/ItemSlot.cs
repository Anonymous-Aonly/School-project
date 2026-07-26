using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemSlot : MonoBehaviour
{
    [Header("Identification")]
    public string CountryName;

    [Header("Colors")]
    public Color defaultColor = new Color(1f, 1f, 1f, 0.3f);
    public Color hoverColor = new Color(0.5f, 0.7f, 1f, 0.6f);   
    public Color filledColor = new Color(0.4f, 0.9f, 0.6f, 0.7f); 

    [Header("Animation")]
    public float colorLerpSpeed = 10f;
    public float pulseAmount = 0.1f;

    [Header("Size & Zoom Settings")]
    [Tooltip("The base size of the slot when the map is at 100% zoom")]
    public Vector2 targetLocalScale = new Vector2(1f, 1f);
    [Tooltip("Max size limit so it doesn't cover the map when zoomed way out")]
    public Vector2 maxLocalScale = new Vector2(1.5f, 1.5f);
    [Tooltip("Min size limit so it doesn't disappear when zoomed way in")]
    public Vector2 minLocalScale = new Vector2(0.3f, 0.3f);
    [Tooltip("How fast it shrinks when zooming in. 1 = normal, 2 = shrinks twice as fast")]
    public float shrinkIntensity = 1.5f;

    private Image backgroundImage;
    private Color targetColor;
    private DragDrop currentFlag;
    private RectTransform rectTransform;
    private FinalMapZoom mapRect;
    
    // Used to multiply with zoom so the pulse animation doesn't glitch
    private float pulseMultiplier = 1f; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        backgroundImage = GetComponent<Image>();
        targetColor = defaultColor;

        if (backgroundImage != null)
            backgroundImage.color = defaultColor;

        // Automatically find the map by Tag
        GameObject mapObj = GameObject.FindGameObjectWithTag("Map");
        if (mapObj != null)
        {
            mapRect = mapObj.GetComponent<FinalMapZoom>();
        }
    }

    private void Update()
    {
        // Smooth color transition
        if (backgroundImage != null)
        {
            backgroundImage.color = Color.Lerp(backgroundImage.color, targetColor, Time.deltaTime * colorLerpSpeed);
        }

        // Real-time zoom scaling
        AdjustScaleToZoom();
    }

    private void AdjustScaleToZoom()
    {
        if (mapRect == null) return;

        float zoomLevel = mapRect.currentZoom;
        
        // Exponential curve forces it to shrink increasingly more as you zoom in
        float zoomFactor = Mathf.Pow(zoomLevel, shrinkIntensity);
        
        float idealX = targetLocalScale.x / zoomFactor;
        float idealY = targetLocalScale.y / zoomFactor;

        float clampedX = Mathf.Clamp(idealX, minLocalScale.x, maxLocalScale.x);
        float clampedY = Mathf.Clamp(idealY, minLocalScale.y, maxLocalScale.y);

        // Apply zoom scale and multiply by pulse if active
        rectTransform.localScale = new Vector2(clampedX * pulseMultiplier, clampedY * pulseMultiplier);
    }

    public bool HasAnyFlag()
    {
        return currentFlag != null;
    }

    public void Highlight()
    {
        targetColor = hoverColor;
        StartCoroutine(PulseScale());
    }

    public void ResetColor()
    {
        targetColor = HasAnyFlag() ? filledColor : defaultColor;
        pulseMultiplier = 1f; // Reset pulse immediately if mouse leaves
    }

    public void OnFlagDropped()
    {
        currentFlag = GetComponentInChildren<DragDrop>();
        targetColor = filledColor;
    }

    public void RemoveFlag()
    {
        currentFlag = null;
        targetColor = defaultColor;
    }

    private IEnumerator PulseScale()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            pulseMultiplier = 1f + Mathf.Sin(t * Mathf.PI) * pulseAmount;
            yield return null;
        }

        pulseMultiplier = 1f;
    }
}