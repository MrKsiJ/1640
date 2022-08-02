using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

public class TimeController : MonoBehaviour
{
    public static TimeController instance;

    [HideInInspector] public string timePlayingGameSession;
    private TimeSpan timePlaying;
    
    private bool timerGoing;

    internal float elapsedTime;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        timerGoing = false;
    }

    public void BeginTimer(bool startTimerNull = true)
    {
        timerGoing = true;
        if(startTimerNull)
            elapsedTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    public void EndTimer()
    {
        timerGoing = false;
    }

    private IEnumerator UpdateTimer()
    {
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            var timePlayingStr = timePlaying.ToString("hh':'mm':'ss'.'ff");
            timePlayingGameSession = timePlayingStr;
            
            yield return null;
        }
    }

    public void RandomTimer()
    {
        elapsedTime = UnityEngine.Random.Range(1.0f, 999.0f);
        timePlaying = TimeSpan.FromSeconds(elapsedTime);
        var timePlayingStr = timePlaying.ToString("hh':'mm':'ss'.'ff");
        timePlayingGameSession = timePlayingStr;
    }
}
