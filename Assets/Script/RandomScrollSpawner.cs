using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class RandomScrollSpawner : MonoBehaviour
{
    public Transform content;
    public GameObject[] prefabs;
    public int maxCount = 10;

    // Expose the list so GameManager can read it
    public List<DragDrop> SpawnedDragDrops { get; private set; } = new List<DragDrop>();

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        SpawnedDragDrops.Clear();

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        GameObject[] uniquePrefabs = prefabs.Distinct().ToArray();
        GameObject[] shuffled = uniquePrefabs.OrderBy(x => Random.value).ToArray();

        int count = Mathf.Min(maxCount, shuffled.Length);

        for (int i = 0; i < count; i++)
        {
            GameObject spawned = Instantiate(shuffled[i], content);

            Image img = spawned.GetComponentInChildren<Image>();
            if (img != null) img.raycastTarget = true;

            DragDrop dd = spawned.GetComponentInChildren<DragDrop>();
            if (dd != null) SpawnedDragDrops.Add(dd);
        }
    }
}