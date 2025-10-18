using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    struct inputs
    {
       // bool 
    }

    List<Vector3> positions;
    public Transform player;
    public Transform AI;
    public bool recording;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Start()
    {
        positions = new List<Vector3>();
    }

    void FixedUpdate()
    {
        /*if (recording)
        {
            Record();
        }
        else
        {
            Replay();
        }*/

        /*yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);*/

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.z, 10);
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float angle = Mathf.Atan2(lookPos.z, lookPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.down); // Turns Right
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up); //Turns Left*/

        //positions.Insert(new Vector3(speedH * Input.GetAxis("Mouse X"), Input.mousePosition.z, 0));

    }

    void Record()
    {
        positions.Insert(0, player.position);
    }

    void Replay()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            AI.position = positions[i];
        }
    }
}
