using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TimeUtils{
    public static float TcpSingleDelay = 0;
    private static List<float> TcpDelayList = new List<float>();
    public static float UdpSingleDelay = 0;
    private static List<float> UdpDelayList = new List<float>();

    public static DateTime GetDateTimeByTimeStamp(double timeStamp)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        TimeSpan delta = TimeSpan.FromMilliseconds(timeStamp);
        DateTime curTime = startTime.Add(delta);
        return curTime;
    }

    public static long GetTimeStamp(DateTime curTime)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        TimeSpan delta = curTime.Subtract(startTime);
        return (long)Math.Round(delta.TotalMilliseconds, MidpointRounding.AwayFromZero);
    }

    public static long GetTimeStampNow()
    {
        DateTime d = DateTime.Now;
        return GetTimeStamp(d);
    }

    public static TimeSpan GetTimeDelta(double milliseconds)
    {
        DateTime dt = GetDateTimeByTimeStamp(milliseconds);
        DateTime nowTime = DateTime.Now;
        return dt.Subtract(nowTime);
    }

    public static float GenerateTcpDelayTime(float delay)
    {
        if(TcpDelayList.Count < 10)
        {
            TcpDelayList.Add(delay);
        }
        else
        {
            TcpDelayList.RemoveAt(0);
            TcpDelayList.Add(delay);
        }
        float sumDelay = 0;
        foreach(float delaytime in TcpDelayList)
        {
            sumDelay += delaytime;
        }
        sumDelay /= TcpDelayList.Count;
        TcpSingleDelay = sumDelay;
        return sumDelay;
    }

    public static float GenerateUdpDelayTime(float delay)
    {
        if (UdpDelayList.Count < 10)
        {
            UdpDelayList.Add(delay);
        }
        else
        {
            UdpDelayList.RemoveAt(0);
            UdpDelayList.Add(delay);
        }
        float sumDelay = 0;
        foreach (float delaytime in UdpDelayList)
        {
            sumDelay += delaytime;
        }
        sumDelay /= UdpDelayList.Count;
        UdpSingleDelay = sumDelay;
        return sumDelay;
    }
}
