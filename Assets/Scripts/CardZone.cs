using UnityEngine;

public class CardZone : MonoBehaviour
{
    public GameObject objectToActivateOnEnter;
    public GameObject objectToDeactivateOnExit;
    public CardAccess accessType = CardAccess.A; // Set in Inspector

    [Header("Optional Spotlight")]
    public GameObject spotlightObject; // Assign the spotlight GameObject here

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            if (objectToActivateOnEnter != null)
                objectToActivateOnEnter.SetActive(true);

            // Both real player and clones update their access
            player.currentAccess = accessType;

            // Only the real player updates the UI
            if (player.isRealPlayer)
            {
                player.UpdateCardUI();
            }

            // Change spotlight color based on access type
            if (spotlightObject != null)
            {
                Light light = spotlightObject.GetComponent<Light>();
                if (light != null)
                {
                    switch (accessType)
                    {
                        case CardAccess.A:
                            light.color = Color.red; // Example color for A
                            break;
                        case CardAccess.B:
                            light.color = Color.green; // Example color for B
                            break;
                        case CardAccess.C:
                            light.color = Color.blue; // Example color for C
                            break;
                    }
                }
            }

            Debug.Log(other.name + " got access: " + accessType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            if (objectToDeactivateOnExit != null)
                objectToDeactivateOnExit.SetActive(false);
        }
    }
}