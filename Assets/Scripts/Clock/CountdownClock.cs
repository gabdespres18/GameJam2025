using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class CountdownClock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timertext;
    [SerializeField] float remainingTime;
    bool GameOver = false;
    int heure = 4;

    // Update is called once per frame
    private void Start()
    {
        timertext.color = Color.red;
    }

    void Update()
    {

        if (remainingTime < 3600)
        {
            remainingTime += Time.deltaTime;
        }
        else if (remainingTime >= 3600 && heure == 4)
        {
            heure++;
            GameOver = true;
            Debug.Log("Game over!");
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timertext.text = string.Format(heure + ":{0:00}:{1:00}", minutes, seconds);

    }
}
