using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Player player;
    public List<Clone> clones;
    public List<Transform> spawns;
    public List<CountdownClock> clocks;

    [Header("Door-related GameObjects")]
    public List<GameObject> doorObjects;

    // *** NEW: Reference to the Door Trigger Script (ASSIGN IN INSPECTOR) ***
    [Header("External References")]
    public DoorTriggerActivator elevatorDoorScript;

    private int lastDoorHandled = -1;

    private Dictionary<Clone, IEnumerator> cloneCoroutines = new Dictionary<Clone, IEnumerator>();


    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        foreach (GameObject go in doorObjects)
        {
            if (go != null)
                go.SetActive(false);
        }

        // Ensure all clones start disabled
        foreach (Clone c in clones)
        {
            if (c != null)
            {
                c.gameObject.SetActive(false);
                c.enabled = true;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ForceLevelRestart();
            return;
        }

        HandleGameFlow();
    }

    void HandleGameFlow()
    {
        int numDoor = player.numDoor;
        bool timeOver = false;
        bool playerCol = false;

        for (int i = 0; i < clones.Count && i < numDoor; i++)
        {
            if (clones[i].playerCollision)
                playerCol = true;
        }

        if (clocks.Count > 0 && clocks[0] != null)
            timeOver = clocks[0].timeOver;

        if (timeOver || playerCol)
        {
            ForceLevelRestart();
            return;
        }

        if (player.finishedRecording)
        {
            player.finishedRecording = false;

            // 1. Start all delayed clone spawns 
            for (int i = 0; i < clones.Count && i <= numDoor; i++)
            {
                Clone c = clones[i];
                Transform spawn = spawns[i];

                float delayTime = (numDoor - i + 1) * 3.0f;

                LaunchCloneCoroutine(c, spawn, i, delayTime);
            }

            // 2. Manage Clocks and Doors
            foreach (CountdownClock clk in clocks)
            {
                if (clk != null)
                    clk.ResetTime(3540 - ((numDoor + 1) * 3.0f));
            }

            if (lastDoorHandled >= 0 && lastDoorHandled < doorObjects.Count)
            {
                if (doorObjects[lastDoorHandled] != null)
                    doorObjects[lastDoorHandled].SetActive(false);
            }

            if (numDoor < doorObjects.Count)
            {
                if (doorObjects[numDoor] != null)
                    doorObjects[numDoor].SetActive(true);
            }

            // 3. Reset Player State for the next segment
            lastDoorHandled = numDoor;
            player.numDoor++;
            player.Reset();
        }

        if (player.numDoor >= spawns.Count - 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    // Handles R-key press or Collision/Time Over reset
    void ForceLevelRestart()
    {
        // Stop all active clone coroutines immediately on reset
        StopAllCoroutines();
        cloneCoroutines.Clear();

        player.Reset();
        player.ResetCurrentRecord();

        // *** CRITICAL FIX: Tell the Door to Forget Everyone ***
        if (elevatorDoorScript != null)
        {
            elevatorDoorScript.ForceResetProximity();
        }

        int numDoor = player.numDoor;

        // 1. Reset Clocks
        foreach (CountdownClock clk in clocks)
        {
            if (clk != null)
            {
                clk.ResetTime(3540 - (numDoor * 3.0f));
                clk.timeOver = false;
            }
        }

        // 2. RE-LAUNCH DELAYED SPAWNING FOR ALL PREVIOUSLY COMPLETED CLONES
        for (int i = 0; i < clones.Count && i < numDoor; i++)
        {
            Clone c = clones[i];
            Transform spawn = spawns[i];

            float delayTime = (numDoor - i) * 3.0f;

            LaunchCloneCoroutine(c, spawn, i, delayTime);
        }
    }

    void LaunchCloneCoroutine(Clone c, Transform spawn, int index, float delayTime)
    {
        if (cloneCoroutines.ContainsKey(c))
        {
            cloneCoroutines.Remove(c);
        }

        // Create the new coroutine instance
        IEnumerator newCoroutine = SpawnCloneWithDelay(c, spawn, index, delayTime);

        // Store the reference and start the coroutine
        cloneCoroutines.Add(c, newCoroutine);
        StartCoroutine(newCoroutine);
    }


    private IEnumerator SpawnCloneWithDelay(Clone c, Transform spawn, int index, float delayTime)
    {
        // 1. Setup the clone and prepare for reset
        c.inputs = new List<Inputs>(player.clones[index].inputs);
        c.initPos = spawn;
        c.currentDoor = index;
        c.InitializeAccess(player.clones[index].accessAtStart);

        // Ensure the clone is off and disabled for the reset process
        c.gameObject.SetActive(false);

        // --- Rigidbody Setup ---
        // Applying the user's positional fix (0, 0, 0 offset)
        Vector3 newPos = spawn.position + new Vector3(0, 0, 0);
        Quaternion newRot = spawn.rotation;

        // CRITICAL FIX: ITERATIVE COLLIDER BLACKOUT
        if (c.allColliders != null)
        {
            foreach (Collider col in c.allColliders)
            {
                if (col != null)
                {
                    col.enabled = false;
                }
            }
        }

        if (c.rb != null)
        {
            c.rb.linearVelocity = Vector3.zero;
            c.rb.angularVelocity = Vector3.zero;
            c.rb.isKinematic = true;
        }

        // Reposition and stabilize the clone
        c.transform.position = newPos;
        c.transform.rotation = newRot;

        // Manually set the Rigidbody position to kill race condition
        if (c.rb != null)
        {
            c.rb.position = newPos;
            c.rb.rotation = newRot;
        }

        // Call Reset() to clear internal flags
        c.Reset();

        // A micro-delay to ensure Unity processes the position change
        yield return null;

        // 2. Robust Manual Countdown Timer (The Clone is invisible and inactive here)
        float timer = delayTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // 3. Activation and Start Replay
        c.gameObject.SetActive(true);

        // Re-enable Collider/Rigidbody Interaction
        if (c.allColliders != null)
        {
            foreach (Collider col in c.allColliders)
            {
                if (col != null)
                {
                    col.enabled = true;
                }
            }
        }

        if (c.rb != null)
        {
            c.rb.isKinematic = false;
        }

        c.StartReplay();

        // 4. Clean up the coroutine reference
        if (cloneCoroutines.ContainsKey(c))
        {
            cloneCoroutines.Remove(c);
        }
    }
}