using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Util.Debugger
{
    public class CompactLog : MonoBehaviour
    {
        static Dictionary<string, List<object>> logs = new Dictionary<string, List<object>>();

        public static void Log(string key, object value)
        {
            if (!logs.ContainsKey(key))
            {
                logs.Add(key, new List<object>());
            }

            logs[key].Add(value);
        }

        public static void WriteLogToConsole(string key)
        {
            if (logs.ContainsKey(key))
            {
                List<object> list = logs[key];
                string output = "Log for Key:" + key + "\n";
                foreach(object o in list)
                {
                    output += o + "\n";
                }
                Debug.Log(output);
                logs.Remove(key);
            }
        }
    }
}
