using UnityEngine;

public class CompassSpin : MonoBehaviour
{
    [Tooltip("How fast it spins in degrees per second")]
    [SerializeField] private float rotationSpeed = 50f;

    void Update()
    {
        transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
    }
}