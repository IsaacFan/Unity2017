using UnityEngine;
using System;
using System.Collections.Generic;

public class TimerManager {

    private static TimerManager instance;
    public static TimerManager Instace
    {
        get
        {
            if (instance == null)
                instance = new TimerManager();

            return instance;
        }
    }


    //private float currentTime;
    public float CurrentTime
    {
        get { return Time.time; }
    }

    private int maxSerialID;
    private Dictionary<int, Timer> timerList = new Dictionary<int, Timer>();
    private List<Timer> recycledTimerList = new List<Timer>();
    private Dictionary<int, Timer>.Enumerator timerListIter;
    private bool getNextLink;
    private List<int> deleteList = new List<int>();


    public void init()
    {
        GameCore.Instace.registerUpdateAction(update);
    }
    public void destroy()
    {
        GameCore.Instace.unregisterUpdateAction(update);

        timerList.Clear();
        recycledTimerList.Clear();
        deleteList.Clear();
    }

    private void update()
    {
        if (timerList.Count == 0)
            return;

        deleteList.Clear();

        timerListIter = timerList.GetEnumerator();
        getNextLink = timerListIter.MoveNext();
        while (getNextLink == true)
        {
            KeyValuePair<int, Timer> pair = timerListIter.Current;
            // try to get next link point, must before executeCallbackFunc()
            getNextLink = timerListIter.MoveNext();

            if (pair.Value.isPause == true)
                continue;

            if (pair.Value.isTimeUp() == true)
            {           
                pair.Value.executeCallbackFunction();
                if (pair.Value.getLeftRepeatCount() == 0)
                    deleteList.Add(pair.Key);
            }
        }

        for (int i = 0; i < deleteList.Count; i++)
            timerList.Remove(deleteList[i]);
    }

    public Timer createTimer(int repeatCount, float interval, float delay, Action callbackAction)
    {
        Timer timer;
        if (recycledTimerList.Count > 0)
        {
            timer = recycledTimerList[recycledTimerList.Count - 1];
            recycledTimerList.RemoveAt(recycledTimerList.Count - 1);
        }
        else
        {
            timer = new Timer();
        }

        timer.init(maxSerialID, repeatCount, interval, delay, callbackAction);  
        timerList.Add(maxSerialID, timer);
        maxSerialID++;

        return timer;
    }

    public bool deleteTimer(Timer timer)
    {
        return deleteTimer(timer.SerialID);
    }
    public bool deleteTimer(int serialID)
    {
        if (timerList.ContainsKey(serialID) == false)
            return false;

        recycledTimerList.Add(timerList[serialID]);
        timerList.Remove(serialID);
        return true;
    }

    public void removeAllTimer()
    {
        for (int i = 0; i < timerList.Count; i++)
            recycledTimerList.Add(timerList[i]);

        timerList.Clear();
    }

}

public class Timer
{
    public int SerialID;
    private int repeatCount;
    private float interval;
    private float timeLimit;
    private Action callbackAction;
    public bool isPause;

    public void init(int serialID, int repeatCount, float interval, float delay, Action callbackAction)
    {
        SerialID = serialID;
        this.repeatCount = repeatCount;
        this.interval = interval;
        timeLimit = TimerManager.Instace.CurrentTime + delay;
        this.callbackAction = callbackAction;
        isPause = false;
    }

    public void executeCallbackFunction()
    {
        if (callbackAction != null)
            callbackAction();

        if (repeatCount != 0)
        {
            calculateTimeLimit();

            if (repeatCount > 0)
                repeatCount--;
        }
    }

    void calculateTimeLimit()
    {
        timeLimit += interval;
    }

    public void resetTimeLimit()
    {
        timeLimit = TimerManager.Instace.CurrentTime + interval;
    }

    public bool isLoop()
    {
        return repeatCount == -1;
    }
    public bool isTimeUp()
    {
        return timeLimit < TimerManager.Instace.CurrentTime;
    }

    public float getCountDownTime()
    {
        return timeLimit - TimerManager.Instace.CurrentTime;
    }
    public int getLeftRepeatCount()
    {
        return repeatCount;
    }


}