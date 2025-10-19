using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Player player;
    public List<Clone> clones;
    public List<Transform> spawns;
    public List<CountdownClock> clocks;

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

    void FixedUpdate()
    {
        int numDoor = player.numDoor;
        bool timeOver = false;
        bool playerCol = false;

        for (int i = 0; i < clones.Count && i < numDoor; i++)
        {
            if (clones[i].playerCollision)
                playerCol = true;
        }

        if (clocks[0] != null)
            timeOver = clocks[0].timeOver;

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

                c.InitializeAccess(player.currentAccess); // Inherit access
                
                c.transform.position = spawn.position + new Vector3(0, 0.91f, 0);
                c.transform.rotation = spawn.rotation;

                c.gameObject.SetActive(true);
                c.Reset();
            }

            foreach (CountdownClock clk in clocks)
            {
                if (clk != null)
                    clk.ResetTime(3540 - ((numDoor + 1) * 3.0f));
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
        if (Input.GetKeyDown(KeyCode.R) || timeOver || playerCol)
        {
            player.Reset();
            player.ResetCurrentRecord();
            for (int i = 0; i < clones.Count && i < numDoor; i++)
            {
                Clone c = clones[i];

                c.gameObject.SetActive(true);
                c.Reset();
            }

            foreach (CountdownClock clk in clocks)
            {
                if (clk != null)
                {
                    clk.ResetTime(3540-(numDoor * 3.0f));
                    clk.timeOver = false;
                }
            }
        }

        // End level
        if (player.numDoor >= spawns.Count - 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        
    }
}