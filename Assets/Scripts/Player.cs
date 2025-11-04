using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Inputs
{
    public bool w;
    public bool a;
    public bool s;
    public bool d;

    public bool shift;
    public bool ctrl;
    public bool interact;

    // We record the calculated rotations instead of raw mouse input for better replay accuracy
    public float RotLeftRight;
    public float RotUpDown;
    public float CameraXRotation; // New: Record the final camera pitch angle

    public Inputs(bool w, bool a, bool s, bool d, bool shift, bool ctrl, bool interact, float rotLR, float rotUD, float camXRot)
    {
        this.w = w;
        this.a = a;
        this.s = s;
        this.d = d;
        this.shift = shift;
        this.ctrl = ctrl;
        this.interact = interact;
        this.RotLeftRight = rotLR;
        this.RotUpDown = rotUD;
        this.CameraXRotation = camXRot;
    }
}

// Renamed Clones to RecordedSegment for clarity
public struct RecordedSegment
{
    public List<Inputs> inputs;
    public Transform initialPos;

    public RecordedSegment(Transform pos)
    {
        this.inputs = new List<Inputs>();
        this.initialPos = pos;
    }
}

public enum CardAccess
{
    A,
    B,
    C
}

public class Player : MonoBehaviour
{
    // public Transform player; // Removed: Unused, transform.parent is used instead
    public List<RecordedSegment> clones; // Renamed from 'clones'
    public List<Transform> spawns;
    public bool record;
    public bool startReplay;

    public float movementSprint = 5.0f;
    public float movementWalk = 2.0f;
    public float mouseSens = 10.0f;

    public bool reset = false;
    public bool finishedRecording;
    public int numDoor = 0;

    public bool IsWalking;
    public bool IsRunning;
    public bool IsLeftTurn;
    public bool IsRightTurn;

    public Animator animator;

    private float mouseX;
    private float mouseY;

    private bool recording;
    private float rotLeftRight;
    private float rotUpDown;
    private float xRotation = 0f;

    private int nbClones = 0;
    private int i;
    private int j;
    private float multiplier = 0.0f;

    public bool isRealPlayer = true;

    public CardAccess currentAccess = CardAccess.A; // Start with A

    [Header("Card UI Objects")]
    public GameObject cardA_UI;
    public GameObject cardB_UI;
    public GameObject cardC_UI;

    [Header("Card UI Objects")]
    [SerializeField] private AudioSource step1;
    [SerializeField] private AudioSource step2;


    void Start()
    {
        //initPos = transform;
        transform.parent.position = spawns[numDoor].position + new Vector3(0, 0f, 0);
        transform.parent.rotation = spawns[numDoor].rotation;

        clones = new List<RecordedSegment>(); // Changed type to RecordedSegment

        record = true;

        animator = GetComponentInChildren<Animator>();

        //Screen.lockCursor = true;

        i = 0;

        UpdateCardUI(); // Ensure UI starts correctly
    }

    public void UpdateCardUI()
    {
        if (!isRealPlayer) return; // Only the real player updates UI
        cardA_UI.SetActive(currentAccess == CardAccess.A);
        cardB_UI.SetActive(currentAccess == CardAccess.B);
        cardC_UI.SetActive(currentAccess == CardAccess.C);
    }

    void FixedUpdate()
    {
        Vector3 movement = Vector3.zero;

        // Reset rotation values before reading input
        rotLeftRight = 0f;
        rotUpDown = 0f;

        if (!SceneLoader.IsPaused)
        {
            /********** Live Input Reading **********/

            // Movement Input
            if (Input.GetKey(KeyCode.S)) { movement += Vector3.back; }
            if (Input.GetKey(KeyCode.W)) { movement += Vector3.forward; }
            if (Input.GetKey(KeyCode.D)) { movement += Vector3.right; }
            if (Input.GetKey(KeyCode.A)) { movement += Vector3.left; }

            // Speed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                multiplier = movementSprint;
                IsRunning = true;
            }
            else
            {
                multiplier = movementWalk;
                IsRunning = false;
            }

            // Walking status
            IsWalking = movement != Vector3.zero;

            // Mouse Input
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            rotLeftRight = mouseX * mouseSens;
            rotUpDown = mouseY * mouseSens;
        }

        /********** Movement Calculation **********/

        // Apply movement
        transform.parent.Translate(movement * multiplier * Time.deltaTime, Space.Self);

        // Apply horizontal rotation (player/parent rotation)
        transform.parent.Rotate(0, rotLeftRight, 0);

        // Apply vertical rotation (camera/child rotation)
        xRotation -= rotUpDown;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


        /********** Record **********/

        if (record)
        {
            if (!recording)
            {
                clones.Add(new RecordedSegment(spawns[numDoor])); // Changed struct name
                recording = true;
            }

            // Record all necessary inputs and calculated rotation values for precise replay
            clones[numDoor].inputs.Add(new Inputs(
                Input.GetKey(KeyCode.W),
                Input.GetKey(KeyCode.A),
                Input.GetKey(KeyCode.S),
                Input.GetKey(KeyCode.D),
                Input.GetKey(KeyCode.LeftShift),
                Input.GetKey(KeyCode.LeftControl),
                Input.GetKey(KeyCode.Mouse0),
                rotLeftRight, // Record the actual rotation applied
                rotUpDown,    // Record the actual rotation applied
                xRotation));  // Record the final camera pitch
            i++;
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
        // Example: Trigger zones that change card access
        CardZone zone = col.GetComponent<CardZone>();
        if (zone != null)
        {
            currentAccess = zone.accessType;
            UpdateCardUI(); // <--- Call added here
            Debug.Log("Player got access: " + currentAccess);
        }

        // Keep your existing door logic
        if (col.gameObject.name == "Spawn" + (numDoor + 1))
        {
            record = false;
            recording = false;
            finishedRecording = true;
        }
    }

    public void Reset()
    {
        transform.parent.position = spawns[numDoor].position + new Vector3(0, 0f, 0);
        transform.parent.rotation = spawns[numDoor].rotation;

        record = true;
        finishedRecording = false;
        i = 0;
        xRotation = 0f; // Reset camera rotation
    }

    public void ResetCurrentRecord()
    {
        clones[numDoor].inputs.Clear();
    }
}