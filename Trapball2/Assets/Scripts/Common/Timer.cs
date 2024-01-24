using System;
using System.Collections;
using UnityEngine;

public class Timer
{
    private long time;
    public long restTime;
    private long cicleTime;
    private Callback callback;
    long startTime;
    long elapsedTime;
    private bool pause = true;
    public bool activated = false;


    public Timer(int time, Callback callback)
    {
        this.time = time;
        this.callback = callback;
    }

    public void startTimer()
    {
        pause = false;
        activated = true;
        startTime = getMiliseconds();
        elapsedTime = 0;
        cicleTime = time;
        restTime = time;
        callback.getMonoBehaviour().StartCoroutine(CicleTimer());
    }

    private IEnumerator CicleTimer()
    {
        yield return new WaitForSeconds(0.001f);
        execCicleTimer();
    }

    public void execCicleTimer()
    {
        if (!pause)
        {
            if (restTime <= 0)
            {
                activated = false;
                callback.shot();
            }
            else
            {
                restTime = cicleTime - (getMiliseconds() - startTime);
                callback.getMonoBehaviour().StartCoroutine(CicleTimer());
            }
        }
    }

    public void stopTimer()
    {
        pause = true;
        activated = false;
    }

    public void pauseTimer()
    {
        if (!pause && activated)
        {
            elapsedTime = getMiliseconds() - startTime;
            pause = true;
        }
    }

    public void resumeTimer()
    {
        if (pause && activated)
        {
            long remainingTime = time - elapsedTime;
            restTime = remainingTime > 0 ? remainingTime : time;
            cicleTime = restTime;
            startTime = getMiliseconds();
            pause = false;
            callback.getMonoBehaviour().StartCoroutine(CicleTimer());
        }
    }

    public bool resumeWithProtection()
    {
        if (pause && activated)
        {
            long remainingTime = time - elapsedTime;
            if (remainingTime > 0)
            {
                startTime = getMiliseconds();
                restTime = remainingTime;
                cicleTime = restTime;
                pause = false;
                callback.getMonoBehaviour().StartCoroutine(CicleTimer());
                return true;
            }
            else
            {
                return false;
            }
        } else
        {
            return false;
        }
    }

    public long getRestTime()
    {
        return restTime;
    }

    public interface Callback
    {
        void shot();
        MonoBehaviour getMonoBehaviour();
    }

    public static long getMiliseconds()
    {
        return (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }

}
