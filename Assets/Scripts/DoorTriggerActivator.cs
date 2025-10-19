using UnityEngine;

public class DoorTriggerActivator : MonoBehaviour
{
    [Header("Assign GameObjects to toggle")]
    public GameObject objectToActivateOnEnter;
    public GameObject objectToDeactivateOnEnter;

    public GameObject objectToActivateOnExit;
    public GameObject objectToDeactivateOnExit;

    [Tooltip("The tag of the object that will trigger this behavior.")]
    public string triggerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            if (objectToActivateOnEnter != null)
                objectToActivateOnEnter.SetActive(true);
            if (objectToDeactivateOnEnter != null)
                objectToDeactivateOnEnter.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            if (objectToActivateOnExit != null)
                objectToActivateOnExit.SetActive(true);
            if (objectToDeactivateOnExit != null)
                objectToDeactivateOnExit.SetActive(false);
        }
    }
}