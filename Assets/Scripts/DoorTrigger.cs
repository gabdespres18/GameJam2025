using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public CardAccess requiredAccess = CardAccess.A; // Set per door
    public GameObject doorObjectOpen;
    public GameObject doorObjectClose;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        Clone clone = other.GetComponent<Clone>();

        if (player != null && player.currentAccess == requiredAccess)
        {
            doorObjectOpen.SetActive(true); // Open door
            doorObjectClose.SetActive(false); // Open door
        }

        if (clone != null && clone.currentAccess == requiredAccess)
        {
            doorObjectOpen.SetActive(true); // Open door
            doorObjectClose.SetActive(false); // Open door
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Optional: reset door when player leaves
        Player player = other.GetComponent<Player>();
        Clone clone = other.GetComponent<Clone>();

        if (player != null || clone != null)
        {
            doorObjectOpen.SetActive(false); // Open door
            doorObjectClose.SetActive(true); // Open door
        }
    }
}