using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public CardAccess requiredAccess = CardAccess.A; // Set per door
    public GameObject doorObject;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        Clone clone = other.GetComponent<Clone>();

        if (player != null && player.currentAccess == requiredAccess)
        {
            doorObject.SetActive(false); // Open door
        }

        if (clone != null && clone.currentAccess == requiredAccess)
        {
            doorObject.SetActive(false); // Open door
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Optional: reset door when player leaves
        Player player = other.GetComponent<Player>();
        Clone clone = other.GetComponent<Clone>();

        if (player != null || clone != null)
        {
            doorObject.SetActive(true); // Close door
        }
    }
}