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
    public GameObject TemplateCountryRightTxtGameObject;
    public GameObject TemplateCountryWrongTxtGameObject;
    public TextMeshProUGUI FlagScore;

    private List<DragDrop> validFlags;

    [SerializeField] private Sprite cursorSprite;
    [SerializeField] private Vector2 hotspot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

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
        // ✅ FOOLPROOF: Finds ALL DragDrop scripts in the scene, 
        // regardless of whether they are in the scroll view, being dragged, or in a map slot!
        DragDrop[] allDragDrops = FindObjectsOfType<DragDrop>();

        if (allDragDrops.Length == 0)
        {
            Debug.LogWarning("No DragDrop scripts found in the scene!");
            return;
        }

        // Set our list to whatever was found
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
            if (item.IsValid)
            {
                GameObject newGameObject = Instantiate(TemplateCountryRightTxtGameObject);
                newGameObject.transform.SetParent(TemplateCountryRightTxtGameObject.transform.parent);
                newGameObject.GetComponent<TextMeshProUGUI>().text = item.CountryName;

                RectTransform rt = newGameObject.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                rt.sizeDelta = Vector2.zero;
            }
            else
            {
                GameObject newGameObject = Instantiate(TemplateCountryWrongTxtGameObject);
                newGameObject.transform.SetParent(TemplateCountryWrongTxtGameObject.transform.parent);
                newGameObject.GetComponent<TextMeshProUGUI>().text = item.CountryName;

                RectTransform rt = newGameObject.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                rt.sizeDelta = Vector2.zero;
            }
        }

        TemplateCountryRightTxtGameObject.SetActive(false);
        TemplateCountryWrongTxtGameObject.SetActive(false);
        FlagScore.text = validFlags.Count + "/" + FlagList.Length;
    }

    private void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}