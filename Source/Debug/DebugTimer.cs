using System;
using System.Collections;
using System.Collections.Generic;

namespace NoUtil.Debugging
{
    public static class DebugTime
    {
        static Dictionary<string, DateTime> logtime;
        public static void Log(string LogKey)
        {
            if (logtime == null)
            {
                logtime = new Dictionary<string, DateTime>();
            }

            DateTime currentTime = DateTime.Now;
            if (logtime.ContainsKey(LogKey))
            {
                Debugger.Log(LogKey, "Time Elapsed" + (logtime[LogKey] - currentTime).Milliseconds + "ms");
                logtime[LogKey] = currentTime;
            }
            else
            {
                logtime.Add(LogKey, currentTime);
            }
        }
    }
}