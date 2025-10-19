using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Player player;
    public List<Clone> clones;
    public List<Transform> spawns;

    [Header("Door-related GameObjects")]
    [Tooltip("Assign a GameObject for each door in order.")]
    public List<GameObject> doorObjects;

    private int lastDoorHandled = -1; // track last door player finished

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        // Deactivate all door objects at start
        foreach (GameObject go in doorObjects)
        {
            if (go != null)
                go.SetActive(false);
        }
    }

    void Update()
    {
        int numDoor = player.numDoor;

        // Handle clones when player finishes a door
        if (player.finishedRecording)
        {
            // Reset and assign clones for this door
            for (int i = 0; i < clones.Count && i <= numDoor; i++)
            {
                Clone c = clones[i];
                Transform spawn = spawns[i];

                c.inputs = new List<Inputs>(player.clones[i].inputs);
                c.initPos = spawn;
                c.currentDoor = i;
                c.waitTime = (numDoor - i + 1) * 3.0f;

                c.transform.position = spawn.position + new Vector3(0, 0.91f, 0);
                c.transform.rotation = spawn.rotation;

                c.gameObject.SetActive(true);
                c.Reset();
            }

            // Deactivate previous door object
            if (lastDoorHandled >= 0 && lastDoorHandled < doorObjects.Count)
            {
                if (doorObjects[lastDoorHandled] != null)
                    doorObjects[lastDoorHandled].SetActive(false);
            }

            // Activate the next door object (corresponding to the door just reached)
            if (numDoor < doorObjects.Count)
            {
                if (doorObjects[numDoor] != null)
                    doorObjects[numDoor].SetActive(true);
            }

            lastDoorHandled = numDoor; // update last handled
            player.numDoor++;
            player.Reset();
        }

        // R-key reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            player.Reset();
            foreach (Clone c in clones)
            {
                if (c.gameObject.activeSelf)
                    c.Reset();
            }
        }

        // End level
        if (player.numDoor >= spawns.Count - 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}