using UnityEngine;
using System;


public static class TimeSpanExtension 
{
    public static string ToHHMMSS(this TimeSpan timeSpan)
    {
        string result = timeSpan.TotalSeconds > 0 ?
            string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds) :
            "00:00:00";
        
        return result;
    }


    public static string ToHHMM(this TimeSpan timeSpan)
    {
        string result = timeSpan.TotalSeconds > 0 ?
            string.Format("{0:D2}:{1:D2}", timeSpan.Hours, timeSpan.Minutes) :
            "00:00";

        return result;
    }


    public static string ToMMSS(this TimeSpan timeSpan)
    {
        string result = timeSpan.TotalSeconds > 0 ?
            string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds) :
            "00:00";

        return result;
    }


    public static string ToTimerString(this TimeSpan timeSpan)
    {
        int minutes = timeSpan.Hours * 60 + timeSpan.Minutes;
        int seconds = timeSpan.Seconds;

        return string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }


    public static string ToShortTimeString(this TimeSpan timeSpan)
    {
        int hours = timeSpan.Days * 24 + timeSpan.Hours;

        return string.Format("{0}:{1:D2}:{2:D2}", hours, timeSpan.Minutes, timeSpan.Seconds);
    }
}