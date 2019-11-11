using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Stars;

public class TimerManager : SingletonMonoBehaviour<TimerManager>
{
    public List<Timer> Timers = new List<Timer>();

    public Timer GenerateTimer(float time)
    {
        Timer timer = new Timer(time);
        Timers.Add(timer);
        return timer;
    }

    public void RemoveTimer(Timer timer)
    {
        if (timer == null)
            return;
        if (Timers.Contains(timer))
        {
            timer.Stop();
            Timers.Remove(timer);
            timer = null;
        }
    }

    void Update()
    {
        for(int i = 0; i < Timers.Count; ++i)
        {
            Timers[i].OnUpdate(Time.deltaTime);
        }
    }

    public void StopTimers()
    {
        for(int i = 0; i < Timers.Count; ++i)
        {
            Timers[i].Stop();
        }
    }

    public void ContinueTimers()
    {
        for (int i = 0; i < Timers.Count; ++i)
        {
            Timers[i].Continue();
        }
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }
    //秒数转为00:00
    public string SecondsChangeInto(int secInt)
    {
        int DDD = secInt / (60 * 60 * 24);
        int HH = (secInt % (60 * 60 * 24)) / (60 * 60);
        int mm = ((secInt % (60 * 60 * 24)) % (60 * 60)) / 60;
        int ss = secInt % 60;
        string str = "";
        if (DDD > 0)
        {
            str += (":");
        }
        if (HH > 0)
        {
            str += HH < 10 ? "0" : "";
            str += HH + ":";
        }
        if (mm > 0)
        {
            str += mm < 10 ? "0" : "";
            str += mm + ":";
        }
        else
        {
            str += "00:";
        }
        if (ss > 0)
        {
            str += ss < 10 ? "0" : "";
            str += ss;
        }
        else
        {
            str += "00";
        }

        return str;
    }
}