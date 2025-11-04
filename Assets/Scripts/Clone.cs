using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Note: Ensure Inputs and CardAccess structs/enums are accessible.

public class Clone : MonoBehaviour
{
    public List<Inputs> inputs;

    public Transform initPos;
    public bool startReplay = false;

    public float movementSprint = 5.0f;
    public float movementWalk = 2.0f;
    public float mouseSens = 10.0f;

    public int currentDoor;

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

    public CardAccess currentAccess = CardAccess.A;

    public bool playerCollision = false;

    public Transform cloneHead;

    public Rigidbody rb;
    public Collider mainCollider;
    public List<Collider> allColliders; // *** CRITICAL: List to hold all colliders (for Game Manager to disable) ***

    [Header("Sounds")]
    [SerializeField] private AudioSource step1;
    [SerializeField] private AudioSource step2;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();

        if (cloneHead == null && transform.childCount > 0)
        {
            cloneHead = transform.GetChild(0);
        }

        // CRITICAL: Get ALL colliders in the hierarchy, including inactive ones.
        allColliders = new List<Collider>(GetComponentsInChildren<Collider>(true));

        i = 0;
        j = 0;
    }

    void FixedUpdate()
    {
        Vector3 movement = Vector3.zero;

        rotLeftRight = 0f;
        IsWalking = false;
        IsRunning = false;

        /********** Replay **********/
        if (startReplay)
        {
            if (j <= inputs.Count - 1)
            {
                if (inputs[j].s) { movement += Vector3.back; }
                if (inputs[j].w) { movement += Vector3.forward; }
                if (inputs[j].d) { movement += Vector3.right; }
                if (inputs[j].a) { movement += Vector3.left; }

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
                IsWalking = movement != Vector3.zero;

                rotLeftRight = inputs[j].RotLeftRight;
                xRotation = inputs[j].CameraXRotation;

                /********** Movement **********/
                transform.Translate(movement * multiplier * Time.deltaTime, Space.Self);
                transform.Rotate(0, rotLeftRight, 0);

                if (cloneHead != null)
                {
                    float finalXRotation = xRotation;
                    if (j < 10)
                    {
                        finalXRotation = 0f;
                    }
                    cloneHead.localRotation = Quaternion.Euler(finalXRotation, 0f, 0f);
                }

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
        if (col.gameObject.name == "PlayerCollision")
            playerCollision = true;
    }

    public void Reset()
    {
        // Physics is fully managed by GameManager. This just clears internal state.
        if (cloneHead != null)
            cloneHead.localRotation = Quaternion.identity;

        // Fix the first few recorded inputs
        if (inputs != null)
        {
            for (int k = 0; k < Mathf.Min(inputs.Count, 5); k++)
            {
                Inputs inputToFix = inputs[k];
                inputToFix.CameraXRotation = 0f;
                inputs[k] = inputToFix;
            }
        }

        startReplay = false;
        playerCollision = false;
        j = 0;
    }

    public void StartReplay()
    {
        startReplay = true;
    }

    public void InitializeAccess(CardAccess access)
    {
        currentAccess = access;
    }
}