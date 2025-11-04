using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Assuming the Inputs struct is copied or referenced from Player.cs
// (Ensure Inputs is a struct or class with the fields used below)

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
    private float xRotation = 0f; // This will now hold the recorded camera pitch

    private int i;
    private int j;
    private float multiplier = 0.0f;

    public CardAccess currentAccess = CardAccess.A; // CardAccess needs to be defined elsewhere

    public bool playerCollision = false;

    // A reference to the clone's 'head' or 'camera' object (should be the HEAD BONE)
    public Transform cloneHead;

    [Header("Sounds")]
    [SerializeField] private AudioSource step1;
    [SerializeField] private AudioSource step2;

    void Start()
    {
        // Set initial position and rotation
        transform.position = initPos.position + new Vector3(0, 0.91f, 0);
        transform.rotation = initPos.rotation;

        animator = GetComponentInChildren<Animator>();

        // Find the head bone if not set in inspector
        if (cloneHead == null)
        {
            // !!! IMPORTANT: YOU MUST REPLACE THIS STRING with the correct path to your character's head bone.
            // Example: "Ch44_nonPBR@Idle/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head"
            Transform headBone = transform.Find("PATH_TO_YOUR_HEAD_BONE_HERE");

            if (headBone != null)
            {
                cloneHead = headBone;
            }
            else
            {
                Debug.LogError("Clone script: Head bone not found. Vertical rotation (looking up/down) will not work correctly.");
                // Fallback (might rotate the whole character mesh, which is usually wrong)
                if (transform.childCount > 0)
                    cloneHead = transform.GetChild(0);
            }
        }

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
                // Read movement input
                if (inputs[j].s) { movement += Vector3.back; }
                if (inputs[j].w) { movement += Vector3.forward; }
                if (inputs[j].d) { movement += Vector3.right; }
                if (inputs[j].a) { movement += Vector3.left; }

                // Speed
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

                // Use the recorded, scaled rotation values directly
                rotLeftRight = inputs[j].RotLeftRight;
                rotUpDown = inputs[j].RotUpDown;
                xRotation = inputs[j].CameraXRotation; // Get the recorded camera pitch

                /********** Movement **********/

                // Apply movement
                transform.Translate(movement * multiplier * Time.deltaTime, Space.Self);

                // Apply horizontal rotation (rotation of the character's body)
                transform.Rotate(0, rotLeftRight, 0);

                // Apply vertical rotation to the designated clone head/camera object (bone)
                if (cloneHead != null)
                {
                    // Directly apply the recorded camera pitch (xRotation)
                    // The 'xRotation' should be in local space (pitch)
                    cloneHead.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
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
        // (Animator logic remains the same)
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
            // Trigger logic here
        }

        if (col.gameObject.name == "PlayerCollision")
            playerCollision = true;
    }

    public void Reset()
    {
        transform.position = initPos.position + new Vector3(0, 0.91f, 0);
        transform.rotation = initPos.rotation;

        // Reset vertical rotation on the clone's head/camera if it exists
        if (cloneHead != null)
            cloneHead.localRotation = Quaternion.identity;

        // *** FIX FOR FACING THE GROUND (and CS1612) ***
        // Zero out the vertical rotation for the first few frames (up to 5)
        // to ensure the clone starts looking straight ahead.
        for (int k = 0; k < Mathf.Min(inputs.Count, 5); k++)
        {
            // 1. Read the struct out
            Inputs inputToFix = inputs[k];

            // 2. Modify the field
            inputToFix.CameraXRotation = 0f;

            // 3. Write the modified struct back
            inputs[k] = inputToFix;
        }
        // ***********************************************

        startReplay = false;
        playerCollision = false;
        j = 0;

        // Use StopCoroutine/StartCoroutine pattern for safety
        StopCoroutine(WaitingForSpawn());
        StartCoroutine(WaitingForSpawn());
    }

    private IEnumerator WaitingForSpawn()
    {
        yield return new WaitForSeconds(waitTime);
        startReplay = true;
    }

    public void InitializeAccess(CardAccess access)
    {
        currentAccess = access;
    }

}