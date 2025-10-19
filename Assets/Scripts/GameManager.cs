using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Player player;

    public List<Clone> clones;
    public List<Transform> spawns;

    private int numDoor;

    /*public Clone clone1;
    public Clone clone2;
    public Clone clone3;
    public Clone clone4;
    public Clone clone5;

    public Transform spawn1;
    public Transform spawn2;
    public Transform spawn3;
    public Transform spawn4;
    public Transform spawn5;*/

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        numDoor = player.numDoor;

        if (player.finishedRecording)
        {
            clones[numDoor].transform.position = spawns[numDoor].transform.position + new Vector3(0, 0.91f, 0);
            clones[numDoor].transform.rotation = spawns[numDoor].transform.rotation;
            clones[numDoor].inputs = player.clones[numDoor].inputs;
            clones[numDoor].gameObject.SetActive(true);
            clones[numDoor].startReplay = true;
            player.finishedRecording = false;
            player.numDoor++;
        }

    }
}
