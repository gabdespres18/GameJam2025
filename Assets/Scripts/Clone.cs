using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Clone : MonoBehaviour
{
    private List<Inputs> inputs;

    public Transform initPos;
    public bool startReplay;

    public float movementSprint = 5.0f;
    public float movementWalk = 2.0f;
    public float mouseSens = 5.0f;

    public bool isWalking;
    public bool isRunning;
    public bool isLeftTurn;
    public bool isRightTurn;

    public Animator animator;

    private float rotLeftRight;
    private float rotUpDown;
    private float xRotation = 0f;

    private int i;
    private int j;
    private float multiplier = 0.0f;



    void Start()
    {
        transform.position = initPos.position;
        transform.rotation = initPos.rotation;

        animator = GetComponent<Animator>();

        //Screen.lockCursor = true;

        i = 0;
    }

    void FixedUpdate()
    {
        Vector3 movement = Vector3.zero;


        /********** Replay **********/

        if (startReplay)
        {
            if (j <= i - 1)
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
                    isRunning = true;
                }
                else
                {
                    multiplier = movementWalk;
                    isRunning = false;
                }
                if (movement != Vector3.zero)
                    isWalking = true;
                else
                    isWalking = false;

                rotLeftRight = inputs[j].MouseX * mouseSens;
                rotUpDown = inputs[j].MouseY * mouseSens;

                j++;
            }
            else
                startReplay = false;
        }


        /********** Movement **********/

        // Change la position
        transform.Translate(movement * multiplier * Time.deltaTime, Space.Self);

        // Change la rotation gauche droite du player
        transform.Rotate(0, rotLeftRight, 0);

        // Change la rotation haut bas de la caméra
        xRotation -= rotUpDown;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


        /********** Animator **********/

        if (rotLeftRight > 0.2f)
        {
            animator.SetBool("isRightTurn", true);
            animator.SetBool("isLeftTurn", false);
            isRightTurn = true;
            isLeftTurn = false;
        }
        else if (rotLeftRight < -0.2f)
        {
            animator.SetBool("isRightTurn", false);
            animator.SetBool("isLeftTurn", true);
            isLeftTurn = true;
            isRightTurn = false;
        }
        else
        {
            animator.SetBool("isRightTurn", false);
            animator.SetBool("isLeftTurn", false);
            isLeftTurn = false;
            isRightTurn = false;
        }

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);


    }

}
