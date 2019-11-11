using UnityEngine;

public class Timer
{
    public bool b_Tricking;

    private float f_StartTime;

    private float f_TriggerTime;

    private bool w_StarWait;

    public delegate void EventHandler();

    public delegate void OnFinish();

    public OnFinish tick;

    private bool b_loop = false;

    public Timer(float second)
    {
        f_StartTime = Time.time;
        f_TriggerTime = second;
    }

    public void Run()
    {
        b_Tricking = true;
    }

    public void Wait()
    {
        w_StarWait = true;
        f_StartTime = Time.time;
    }

    public void OnUpdate(float deltaTime)
    {
        if (b_Tricking)
        {
            if (Time.time - f_StartTime >= f_TriggerTime)
            {
                if (Isloop)
                {
                    b_Tricking = true;
                    f_StartTime += f_TriggerTime;
                }
                else
                {
                    b_Tricking = false;
                }
                if (null != tick)
                    tick();
            }
        }
    }

    public void Stop()
    {
        b_Tricking = false;
        w_StarWait = false;
    }

    public void Continue()
    {
        b_Tricking = true;
    }

    public void Restart()
    {
        b_Tricking = true;
        f_StartTime = Time.time;
    }

    public void ResetTriggerTime(float second)
    {
        f_StartTime = Time.time;
        f_TriggerTime = second;
    }
    //累计时间
    public int CumulativeTimer()
    {
        if (!w_StarWait)
            return -1;
        return (int)(Time.time - f_StartTime);
    }

    public int RemainingTime()
    {
        if (!b_Tricking)
            return (int)f_TriggerTime;
        int remind = (int)(f_TriggerTime - (Time.time - f_StartTime)) + 1;
        remind = Mathf.Max(0, remind);
        return remind;
    }
    public int CurrentTime()
    {
        if (!b_Tricking)
            return (int)f_TriggerTime;
        int remind = (int)(Time.time - f_StartTime) + 1;
        remind = Mathf.Max(0, remind);
        return remind;
    }


    public bool Isloop
    {
        get{ return b_loop; }
        set{ b_loop = value; }
    }
}