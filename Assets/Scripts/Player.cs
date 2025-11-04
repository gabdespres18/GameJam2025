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
    public float CameraXRotation; // Record the final camera pitch angle

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
    public List<RecordedSegment> clones;
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

    [Header("Sounds")]
    [SerializeField] private AudioSource step1;
    [SerializeField] private AudioSource step2;


    void Start()
    {
        // Set player body position and rotation (on the parent object)
        transform.parent.position = spawns[numDoor].position + new Vector3(0, 0f, 0);
        transform.parent.rotation = spawns[numDoor].rotation;

        clones = new List<RecordedSegment>();

        record = true;

        animator = GetComponentInChildren<Animator>();

        // *** FIX: Explicitly initialize the camera rotation for a level gaze ***
        xRotation = 0f; // Ensure the internal pitch variable is 0
        if (Camera.main != null)
        {
            // Apply 0 pitch to the camera's local rotation
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else
        {
            Debug.LogError("Player script could not find the Main Camera! Check your tag.");
        }
        // ************************************************************************

        //Screen.lockCursor = true; // Uncomment if desired

        i = 0;

        UpdateCardUI();
    }

    public void UpdateCardUI()
    {
        if (!isRealPlayer) return;
        if (cardA_UI != null) cardA_UI.SetActive(currentAccess == CardAccess.A);
        if (cardB_UI != null) cardB_UI.SetActive(currentAccess == CardAccess.B);
        if (cardC_UI != null) cardC_UI.SetActive(currentAccess == CardAccess.C);
    }

    void FixedUpdate()
    {
        Vector3 movement = Vector3.zero;

        // Reset rotation values before reading input
        rotLeftRight = 0f;
        rotUpDown = 0f;

        if (!SceneLoader.IsPaused) // Assuming SceneLoader.IsPaused exists
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
        if (Camera.main != null)
        {
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }


        /********** Record **********/

        if (record)
        {
            if (!recording)
            {
                clones.Add(new RecordedSegment(spawns[numDoor]));
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
                rotLeftRight,
                rotUpDown,
                xRotation)); // Record the final camera pitch
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
        // Assuming CardZone exists or this is placeholder logic
        // CardZone zone = col.GetComponent<CardZone>();
        // if (zone != null)
        // {
        //     currentAccess = zone.accessType;
        //     UpdateCardUI(); 
        // }

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

        // Reset camera pitch variable AND apply it immediately
        xRotation = 0f;
        if (Camera.main != null)
        {
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    public void ResetCurrentRecord()
    {
        if (clones.Count > numDoor)
        {
            clones[numDoor].inputs.Clear();
        }
    }
}