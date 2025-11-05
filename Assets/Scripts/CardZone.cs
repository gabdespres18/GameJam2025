using UnityEngine;
using System.Collections.Generic;

public class CardZone : MonoBehaviour
{
    public GameObject objectToActivateOnEnter;
    public GameObject objectToDeactivateOnExit;
    public CardAccess accessType = CardAccess.A; // Set in Inspector

    [Header("Optional Spotlight")]
    public GameObject spotlightObject; // Assign the spotlight GameObject here

    // Track who we've already applied access to (prevents spam in OnTriggerStay)
    private readonly HashSet<Transform> applied = new HashSet<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        ApplyAccessIfRelevant(other);
    }

    // Covers actors that appear already inside the trigger (e.g., clones spawned/enabled in-zone)
    private void OnTriggerStay(Collider other)
    {
        ApplyAccessIfRelevant(other);
    }

    private void OnTriggerExit(Collider other)
    {
        var root = other.transform.root;
        applied.Remove(root);

        var player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            if (objectToDeactivateOnExit != null)
                objectToDeactivateOnExit.SetActive(false);
        }
    }

    private void ApplyAccessIfRelevant(Collider other)
    {
        // Always resolve to the root so child colliders work
        Transform root = other.transform.root;

        // If we've already applied on this root while inside the trigger, skip
        if (applied.Contains(root)) return;

        var player = other.GetComponentInParent<Player>();
        var clone = other.GetComponentInParent<Clone>();

        if (player == null && clone == null) return;

        if (objectToActivateOnEnter != null)
            objectToActivateOnEnter.SetActive(true);

        if (player != null)
        {
            player.currentAccess = accessType;

            // Only the real player updates the UI
            if (player.isRealPlayer)
                player.UpdateCardUI();

            Debug.Log(root.name + " (Player) got access: " + accessType);
        }

        if (clone != null)
        {
            clone.currentAccess = accessType;
            Debug.Log(root.name + " (Clone) got access: " + accessType);
        }

        // Change spotlight color based on access type
        if (spotlightObject != null)
        {
            var light = spotlightObject.GetComponent<Light>();
            if (light != null)
            {
                switch (accessType)
                {
                    case CardAccess.A: light.color = Color.red; break;
                    case CardAccess.B: light.color = Color.green; break;
                    case CardAccess.C: light.color = Color.blue; break;
                }
            }
        }

        // Mark applied to avoid repeating every frame in OnTriggerStay
        applied.Add(root);
    }
}