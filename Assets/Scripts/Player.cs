using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    struct Clone
    {
        public List<Inputs> inputs;
        public Transform initialPos;

        public Clone(Transform pos)
        {
            this.inputs = new List<Inputs>();
            this.initialPos = pos;
        }
    }

    struct Inputs
    {
        public bool w;
        public bool a;
        public bool s;
        public bool d;

        public bool shift;
        public bool ctrl;
        public bool interact;

        public float MouseX;
        public float MouseY;

        public Inputs(bool w, bool a, bool s, bool d, bool shift, bool ctrl, bool interact, float MouseX, float MouseY)
        {
            this.w = w;
            this.a = a;
            this.s = s;
            this.d = d;
            this.shift = shift;
            this.ctrl = ctrl;
            this.interact = interact;
            this.MouseX = MouseX;
            this.MouseY = MouseY;
        }
    }



    List<Vector3> positions;

    public Transform player;
    public Transform initPos;
    public Transform AI;
    public Rigidbody rb;
    public bool record;
    public bool startReplay;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private bool recording;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private List<Clone> clones;
    private int nbClones = 0;
    private int i;
    private int j;

    
    void Start()
    {
        initPos = transform;

        clones = new List<Clone>();
        positions = new List<Vector3>();

        i = 0;
    }

    void FixedUpdate()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.S))
        {
            movement += Vector2.down;
        }

        if (Input.GetKey(KeyCode.W))
        {
            movement += Vector2.up;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement += Vector2.right;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movement += Vector2.left;
        }

        

        if (record && !recording)
        {
            //CreateClone(initPos);

            clones.Add(new Clone(initPos));

            Debug.Log(clones.Count);

            clones[0].inputs.Add(new Inputs(
                Input.GetKey(KeyCode.W), 
                Input.GetKey(KeyCode.A), 
                Input.GetKey(KeyCode.S), 
                Input.GetKey(KeyCode.D), 
                Input.GetKey(KeyCode.LeftShift), 
                Input.GetKey(KeyCode.LeftControl), 
                Input.GetKey(KeyCode.Mouse0),
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y")));
            recording = true;
            i++;
        }
        else if(record && recording)
        {
            clones[0].inputs.Add(new Inputs(
                Input.GetKey(KeyCode.W),
                Input.GetKey(KeyCode.A),
                Input.GetKey(KeyCode.S),
                Input.GetKey(KeyCode.D),
                Input.GetKey(KeyCode.LeftShift),
                Input.GetKey(KeyCode.LeftControl),
                Input.GetKey(KeyCode.Mouse0),
                Input.GetAxis("Mouse X"), //- clones[nbClones].inputs[i - 1].MouseX,
                Input.GetAxis("Mouse Y"))); //- clones[nbClones].inputs[i - 1].MouseY));
            Record();
            i++;
        }
        else if(!record && recording)
        {
            recording = false;
            startReplay = true;

            j = 0;
        }
        else
        {
            /*if (startReplay)
            {
                Replay(i);

            }*/
        }

        if (startReplay)
        {
            if (j <= i-1)
            {
                if (clones[0].inputs[j].s)
                {
                    movement += Vector2.down;
                }

                if (clones[0].inputs[j].w)
                {
                    movement += Vector2.up;
                }

                if (clones[0].inputs[j].d)
                {
                    movement += Vector2.right;
                }

                if (clones[0].inputs[j].a)
                {
                    movement += Vector2.left;
                }

                j++;
            }
            else
                startReplay = false;
        }

        Vector2 velocity = movement.normalized * 2.0f;
        rb.linearVelocity = velocity;
        /*yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);*/


        /*Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.z, 10);
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float angle = Mathf.Atan2(lookPos.z, lookPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.down); // Turns Right
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up); //Turns Left*/

        //positions.Insert(new Vector3(speedH * Input.GetAxis("Mouse X"), Input.mousePosition.z, 0));
    }

    void Record()
    {
        positions.Add(player.position);
    }

    void Replay(int i)
    {
        AI.position = positions[i];
    }

    void CreateClone(Transform pos)
    {
        clones.Add(new Clone(pos));
        nbClones++;
    }
    void AddInput()
    {

    }
}
