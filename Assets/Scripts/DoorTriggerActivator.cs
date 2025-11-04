using UnityEngine;

public class DoorTriggerActivator : MonoBehaviour
{
    [Header("Assign GameObjects to toggle")]
    public GameObject objectToActivateOnEnter1;
    public GameObject objectToActivateOnEnterL;
    public GameObject objectToDeactivateOnEnter;

    public GameObject objectToActivateOnExit;
    public GameObject objectToDeactivateOnExit;
    public GameObject objectToDeactivateOnExitL;

    [Tooltip("The tag of the object that will trigger this behavior.")]
    public string triggerTag = "Player";

    [Header("Sounds")]
    [SerializeField] private AudioSource elevatorSound;

    // *** CRITICAL ADDITION: STATE TRACKING COUNTER ***
    private int entitiesInside = 0;
    // **************************************************

    // Helper method to open the door and play sound
    private void ToggleOnEnter()
    {
        if (objectToActivateOnEnter1 != null)
        {
            objectToActivateOnEnter1.SetActive(true);
            if (elevatorSound != null)
                elevatorSound.Play();
        }
        if (objectToActivateOnEnterL != null)
        {
            objectToActivateOnEnterL.SetActive(true);
        }
        if (objectToDeactivateOnEnter != null)
            objectToDeactivateOnEnter.SetActive(false);
    }

    // Helper method to close the door
    private void ToggleOnExit()
    {
        if (objectToActivateOnExit != null)
        {
            objectToActivateOnExit.SetActive(true);
            if (elevatorSound != null)
                elevatorSound.Play();
        }
        if (objectToDeactivateOnExit != null)
            objectToDeactivateOnExit.SetActive(false);
        if (objectToDeactivateOnExitL != null)
            objectToDeactivateOnExitL.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            // If the count was zero, the door is currently closed, so open it.
            if (entitiesInside == 0)
            {
                ToggleOnEnter();
            }
            entitiesInside++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            entitiesInside--;

            // Safety check: count should never go below zero.
            if (entitiesInside < 0) entitiesInside = 0;

            // If the count drops to zero, the door should close.
            if (entitiesInside == 0)
            {
                ToggleOnExit();
            }
        }
    }

    // *** CRITICAL PUBLIC RESET METHOD FOR GAMEMANAGER ***
    public void ForceResetProximity()
    {
        // Check if the door is currently open due to an entity
        if (entitiesInside > 0)
        {
            // Close the door before wiping the memory
            ToggleOnExit();
        }
        // Force the count to zero to prevent phantom references
        entitiesInside = 0;
    }
}