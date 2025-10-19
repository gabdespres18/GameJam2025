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
            for (int i = 0; i <= numDoor; i++)
            {
                clones[i].gameObject.SetActive(true);
                clones[i].waitTime = (numDoor - i + 1) * 3.0f;
                clones[i].Reset();
                clones[i].transform.position = spawns[i].transform.position + new Vector3(0, 0.91f, 0);
                clones[i].transform.rotation = spawns[i].transform.rotation;
                clones[i].inputs = player.clones[i].inputs;
                clones[i].currentDoor = i;
            }
            player.numDoor++;
            player.Reset();
        }

        if (player.numDoor >= spawns.Count - 1 )
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0); // scene 0 = main menu
        }

    }
}
