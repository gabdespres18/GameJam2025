using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// --- STRUCT DEFINITIONS (CRITICAL) ---
public struct Inputs
{
    public bool w; public bool a; public bool s; public bool d;
    public bool shift; public bool ctrl; public bool interact;
    public float RotLeftRight; public float RotUpDown;
    public float CameraXRotation;

    public Inputs(bool w, bool a, bool s, bool d, bool shift, bool ctrl, bool interact, float rotLR, float rotUD, float camXRot)
    {
        this.w = w; this.a = a; this.s = s; this.d = d;
        this.shift = shift; this.ctrl = ctrl; this.interact = interact;
        this.RotLeftRight = rotLR; this.RotUpDown = rotUD;
        this.CameraXRotation = camXRot;
    }
}

public struct RecordedSegment
{
    public List<Inputs> inputs;
    public Transform initialPos;
    public CardAccess accessAtStart;   // NEW

    public RecordedSegment(Transform pos, CardAccess access)  // CHANGED
    {
        this.inputs = new List<Inputs>();
        this.initialPos = pos;
        this.accessAtStart = access;
    }
}

public enum CardAccess
{
    A,
    B,
    C
}
// -------------------------------------

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
    private float xRotation = 0f; // Tracks camera pitch

    private int i;
    private float multiplier = 0.0f;

    public bool isRealPlayer = true;

    public CardAccess currentAccess = CardAccess.A;

    [Header("Card UI Objects")]
    public GameObject cardA_UI;
    public GameObject cardB_UI;
    public GameObject cardC_UI;

    [Header("Sounds")]
    [SerializeField] private AudioSource step1;
    [SerializeField] private AudioSource step2;


    void Start()
    {
        transform.parent.position = spawns[numDoor].position + new Vector3(0, 0f, 0);
        transform.parent.rotation = spawns[numDoor].rotation;

        clones = new List<RecordedSegment>();
        record = true;
        animator = GetComponentInChildren<Animator>();
        i = 0;

        // Fix camera pitch on start
        xRotation = 0f;
        if (Camera.main != null)
        {
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

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

        rotLeftRight = 0f;
        rotUpDown = 0f;

        if (!SceneLoader.IsPaused)
        {
            /********** Live Input Reading **********/
            if (Input.GetKey(KeyCode.S)) { movement += Vector3.back; }
            if (Input.GetKey(KeyCode.W)) { movement += Vector3.forward; }
            if (Input.GetKey(KeyCode.D)) { movement += Vector3.right; }
            if (Input.GetKey(KeyCode.A)) { movement += Vector3.left; }

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
            IsWalking = movement != Vector3.zero;

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            rotLeftRight = mouseX * mouseSens;
            rotUpDown = mouseY * mouseSens;
        }

        /********** Movement Calculation **********/
        transform.parent.Translate(movement * multiplier * Time.deltaTime, Space.Self);
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
                if (clones.Count <= numDoor)
                {
                    // First time recording this segment
                    clones.Add(new RecordedSegment(spawns[numDoor], currentAccess));
                }
                else
                {
                    // Re-recording same segment: refresh the stored access + spawn, clear inputs
                    var seg = clones[numDoor];
                    seg.initialPos = spawns[numDoor];
                    seg.accessAtStart = currentAccess;
                    seg.inputs.Clear();
                    clones[numDoor] = seg;
                }
                recording = true;
            }

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
                xRotation));
            i++;
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

    public void RestoreAccessAtSegmentStart()
    {
        if (clones != null && clones.Count > numDoor)
        {
            currentAccess = clones[numDoor].accessAtStart;
            UpdateCardUI(); // keep the UI in sync
        }
    }

    void OnTriggerEnter(Collider col)
    {
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