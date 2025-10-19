using UnityEngine;

public class CardZone : MonoBehaviour
{
    public CardAccess accessType = CardAccess.A; // Set in Inspector

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.currentAccess = accessType;
            Debug.Log("Player got access: " + accessType);
        }
    }
}