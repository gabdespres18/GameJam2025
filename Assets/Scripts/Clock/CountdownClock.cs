using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class CountdownClock : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timertext;
    [SerializeField] float remainingTime;
    bool GameOver = false;

    // Update is called once per frame
    private void Start()
    {
        timertext.color = Color.red;
    }

    void Update()
    {

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime < 0)
        {
            remainingTime = 0;
            GameOver = true;
            Debug.Log("Game over!");
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timertext.text=string.Format("{0:00}:{1:00}", minutes, seconds);

    }
}
