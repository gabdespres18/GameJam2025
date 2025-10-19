using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Clone : MonoBehaviour
{
    public List<Inputs> inputs;

    public Transform initPos;
    public bool startReplay;

    public float movementSprint = 5.0f;
    public float movementWalk = 2.0f;
    public float mouseSens = 10.0f;

    public int currentDoor;
    public float waitTime = 0;

    public bool IsWalking;
    public bool IsRunning;
    public bool IsLeftTurn;
    public bool IsRightTurn;

    public Animator animator;

    private float rotLeftRight;
    private float rotUpDown;
    private float xRotation = 0f;

    private int i;
    private int j;
    private float multiplier = 0.0f;



    void Start()
    {
        transform.position = initPos.position + new Vector3(0, 0.91f, 0); ;
        transform.rotation = initPos.rotation;

        animator = GetComponentInChildren<Animator>();

        //Screen.lockCursor = true;

        i = 0;
        j = 0;
    }

    void FixedUpdate()
    {
        Vector3 movement = Vector3.zero;


        /********** Replay **********/

        if (startReplay)
        {
            if (j <= inputs.Count - 1)
            {
                if (inputs[j].s)
                {
                    movement += Vector3.back;
                }

                if (inputs[j].w)
                {
                    movement += Vector3.forward;
                }

                if (inputs[j].d)
                {
                    movement += Vector3.right;
                }

                if (inputs[j].a)
                {
                    movement += Vector3.left;
                }

                if (inputs[j].shift)
                {
                    multiplier = movementSprint;
                    IsRunning = true;
                }
                else
                {
                    multiplier = movementWalk;
                    IsRunning = false;
                }
                if (movement != Vector3.zero)
                    IsWalking = true;
                else
                    IsWalking = false;

                rotLeftRight = inputs[j].MouseX * mouseSens*2;
                rotUpDown = inputs[j].MouseY * mouseSens*2;

                /********** Movement **********/

                // Change la position
                transform.Translate(movement * multiplier * Time.deltaTime, Space.Self);

                // Change la rotation gauche droite du player
                transform.Rotate(0, rotLeftRight, 0);

                // Change la rotation haut bas de la caméra
               // xRotation -= rotUpDown;
              // xRotation = Mathf.Clamp(xRotation, -90f, 90f);
              //  Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

                j++;
            }
            else
            {
                startReplay = false;
                gameObject.SetActive(false);
            }
        }

        /********** Animator **********/

        if (rotLeftRight > 0.2f)
        {
            animator.SetBool("IsRightTurn", true);
            animator.SetBool("IsLeftTurn", false);
            IsRightTurn = true;
            IsLeftTurn = false;
        }
        else if (rotLeftRight < -0.2f)
        {
            animator.SetBool("IsRightTurn", false);
            animator.SetBool("IsLeftTurn", true);
            IsLeftTurn = true;
            IsRightTurn = false;
        }
        else
        {
            animator.SetBool("IsRightTurn", false);
            animator.SetBool("IsLeftTurn", false);
            IsLeftTurn = false;
            IsRightTurn = false;
        }

        animator.SetBool("IsWalking", IsWalking);
        animator.SetBool("IsRunning", IsRunning);


    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "Spawn" + currentDoor)
        {

        }

        if (col.gameObject.name == "test")
            Debug.Log("Ya balls: " + currentDoor);
    }

    public void Reset()
    {
        transform.position = initPos.position + new Vector3(0, 0.91f, 0); ;
        transform.rotation = initPos.rotation;

        startReplay = false;

        j = 0;
        StopCoroutine(WaitingForSpawn());
        StartCoroutine(WaitingForSpawn());
    }

    private IEnumerator WaitingForSpawn()
    {
        yield return new WaitForSeconds(waitTime);
        startReplay = true;
    }

}
