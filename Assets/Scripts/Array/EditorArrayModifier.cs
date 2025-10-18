using UnityEngine;

public class EditorArrayModifier : MonoBehaviour
{
    public GameObject objectToSpawn;
    public int arrayCount = 5;
    public float rotationIncrement = 30f;

    void OnValidate()
    {
        // Destroy existing children to prevent duplicates during updates
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        if (objectToSpawn == null) return;

        for (int i = 0; i < arrayCount; i++)
        {
            GameObject newObject = Instantiate(objectToSpawn, transform.position, Quaternion.identity);
            newObject.transform.parent = this.transform;
            newObject.transform.Rotate(0, i * rotationIncrement, 0);
        }
    }
}