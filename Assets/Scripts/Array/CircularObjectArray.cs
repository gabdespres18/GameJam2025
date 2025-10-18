using UnityEngine;

public class CircularObjectArray : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int arrayCount = 10;
    public float radius = 5f; // Distance from the center
    public float startAngle = 0f; // Starting angle for the first object

    void Start()
    {
        float angleIncrement = 360f / arrayCount;

        for (int i = 0; i < arrayCount; i++)
        {
            float currentAngle = startAngle + (i * angleIncrement);
            float xPos = radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
            float zPos = radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad);

            Vector3 spawnPosition = new Vector3(xPos, 0, zPos); // Assuming rotation on XZ plane

            // Instantiate the object at the calculated position
            GameObject newObject = Instantiate(objectToSpawn, transform.position + spawnPosition, Quaternion.identity);
            newObject.transform.parent = this.transform;

            // Optional: Make objects face outwards from the center
            newObject.transform.LookAt(transform.position);
            newObject.transform.Rotate(0, 180, 0); // Adjust if needed
        }
    }
}