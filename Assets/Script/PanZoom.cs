using UnityEngine;
using UnityEngine.UI;

public class TileMapLayout : MonoBehaviour
{
    [SerializeField] private Sprite[] tileSprites; // assign in order: row0 left-to-right, then row1...
    [SerializeField] private Vector2 tileSize = new Vector2(512, 512);

    private void Start()
    {
        foreach (var sprite in tileSprites)
        {
            GameObject tileObj = new GameObject(sprite.name, typeof(RectTransform), typeof(Image));
            tileObj.transform.SetParent(transform, false);

            var rt = tileObj.GetComponent<RectTransform>();
            rt.sizeDelta = tileSize;

            var img = tileObj.GetComponent<Image>();
            img.sprite = sprite;
            img.raycastTarget = false;
        }
    }
}