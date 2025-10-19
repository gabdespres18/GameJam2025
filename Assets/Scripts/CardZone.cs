using UnityEngine;

public class CardZone : MonoBehaviour
{
    public CardAccess accessType = CardAccess.A; // Set in Inspector

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            // Both real player and clones update their access
            player.currentAccess = accessType;

            // Only the real player updates the UI
            if (player.isRealPlayer)
            {
                player.UpdateCardUI();
            }

            Debug.Log(other.name + " got access: " + accessType);
        }
    }
}