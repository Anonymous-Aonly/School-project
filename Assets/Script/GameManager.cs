using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public DragDrop[] FlagList;
    public GameObject Win;
    public GameObject Lose;
    public GameObject TemplateResultItemGameObject;
    public TextMeshProUGUI FlagScore;
    
    [Header("Buttons")]
    public GameObject resultButton; // <-- DRAG YOUR RESULT BUTTON HERE

    [Header("Icons")]
    [SerializeField] private Sprite tickSprite;
    [SerializeField] private Sprite crossSprite;

    [Header("Icon Settings")]
    [SerializeField] private float iconWidth = 40f;
    [SerializeField] private float iconHeight = 40f;

    [Header("Cursor")]
    [SerializeField] private Sprite cursorSprite;
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    private List<DragDrop> validFlags;

    void Start()
    {
        if (cursorSprite != null)
        {
            Cursor.SetCursor(cursorSprite.texture, hotspot, CursorMode.ForceSoftware);
        }
    }

    void Update()
    {
        // Empty
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SubmitAnswers()
    {
        // 1. INSTANTLY disable the button so it can't be clicked again
        if (resultButton != null)
        {
            resultButton.SetActive(false);
        }

        DragDrop[] allDragDrops = FindObjectsOfType<DragDrop>();

        if (allDragDrops.Length == 0)
        {
            Debug.LogWarning("No DragDrop scripts found in the scene!");
            return;
        }

        FlagList = allDragDrops;
        validFlags = new List<DragDrop>();
        bool didWeWin = true;

        foreach (var item in FlagList)
        {
            if (item.IsValid != true)
            {
                didWeWin = false;
            }
            else
            {
                validFlags.Add(item);
            }
        }

        if (didWeWin)
        {
            Win.gameObject.SetActive(true);
        }
        else
        {
            Lose.gameObject.SetActive(true);
        }

        foreach (var item in FlagList)
        {
            GameObject newItem = Instantiate(TemplateResultItemGameObject);
            newItem.transform.SetParent(TemplateResultItemGameObject.transform.parent);
            newItem.SetActive(true);

            // Find and update the country name text
            TextMeshProUGUI[] textComponents = newItem.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var txt in textComponents)
            {
                txt.text = item.CountryName;
            }

            // Find and update the icon (tick or cross)
            Image[] images = newItem.GetComponentsInChildren<Image>();
            foreach (var img in images)
            {
                // Skip the template's own image if any, only target child icons
                if (img.transform != newItem.transform)
                {
                    img.sprite = item.IsValid ? tickSprite : crossSprite;
                    
                    // Set icon size
                    RectTransform iconRect = img.GetComponent<RectTransform>();
                    if (iconRect != null)
                    {
                        iconRect.sizeDelta = new Vector2(iconWidth, iconHeight);
                    }
                }
            }

            // Reset RectTransform properly for vertical list
            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
            rt.sizeDelta = Vector2.zero;
        }

        TemplateResultItemGameObject.SetActive(false);
        FlagScore.text = validFlags.Count + "/" + FlagList.Length;
    }

    private void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}