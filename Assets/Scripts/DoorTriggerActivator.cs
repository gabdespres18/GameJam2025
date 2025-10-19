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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
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
                if (elevatorSound != null)
                    elevatorSound.Play();
            }
            if (objectToDeactivateOnEnter != null)
                objectToDeactivateOnEnter.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(triggerTag))
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
    }
}