using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class CountdownClock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timertext;
    [SerializeField] float remainingTime;
    public bool timeOver = false;

    [Header("Sounds")]
    [SerializeField] private AudioSource clockSound;
    
    int heure = 4;
    private float time;

    // Update is called once per frame
    private void Start()
    {
        timertext.color = Color.red;
        ResetTime(remainingTime);

        if (clockSound != null )
            InvokeRepeating("TickTock", 0.6f, 25f);
    }

    void FixedUpdate()
    {

        if (time < 3600)
        {
            time += Time.deltaTime;
        }
        else if (time >= 3600 && heure == 4)
        {
            heure++;
            timeOver = true;
            Debug.Log("Game over!");
        }
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timertext.text = string.Format(heure + ":{0:00}:{1:00}", minutes, seconds);

    }

    public void ResetTime(float t)
    {
        time = t;
    }

    void TickTock()
    {
        clockSound.Play();
    }
}
