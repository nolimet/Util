//Author Jesse D. Stam
//License - MIT License
//Source https://github.com/nolimet/Util/blob/master/Debug/ValueDebugger.cs

using System;
using UnityEngine;

namespace NoUtil.Debugging
{
    /// <summary>
    /// On screen debugger call class
    /// </summary>
    public static class Debugger
    {
        public static event Action<bool> DebugEnabledChanged;
        public static event Action<bool> LogToConsoleChanged;
        public static event Action ClearedDisplay;

        private static bool debugEnabled = true;
        private static bool logToConsole = false;

        public static bool DebugEnabled
        {
            get => debugEnabled; 
            
            set
            {
                debugEnabled = value;
                DebugEnabledChanged?.Invoke(value);
            }
        }
        public static bool LogToConsole
        {
            get => logToConsole; 

            set
            {
                logToConsole = value;
                LogToConsoleChanged?.Invoke(value);
            }
        }

        public static void ClearDisplay()
        {
            ClearedDisplay?.Invoke();
        }

        /// <summary>
        /// Value that will be logged. Also create the object that will be rendered onscreen
        /// completely by code so it does not need a prefab
        /// </summary>
        /// <param name="name">Value's name so you can find it back</param>
        /// <param name="value">Value of the object</param>
        public static void Log(string name, object value)
        {
            if (DebugEnabled)
            {
                ValueDebugger.ValueLog(name, value);
            }
        }

        public static T QuickLog<T>(this T value, string name)
        {
            if (DebugEnabled)
            {
                ValueDebugger.ValueLog(name, value);
            }

            if (LogToConsole)
            {
                QuickCLog(value, name);
            }

            return value;
        }

        public static T QuickLog<T>(this T value, string name, Color nameColor)
        {
            if (DebugEnabled)
            {
                ValueDebugger.ValueLog(name.QuickColor(nameColor), value);
            }

            if (LogToConsole)
            {
                QuickCLog(value, name, nameColor);
            }

            return value;
        }

        public static T QuickLog<T>(this T value, Color valueColor, string name, Color nameColor)
        {
            if (DebugEnabled)
            {
                ValueDebugger.ValueLog(name.QuickColor(nameColor), ToStringNull(value).QuickColor(valueColor));
            }

            if (LogToConsole)
            {
                QuickCLog(value, valueColor, name, nameColor);
            }

            return value;
        }

        public static T QuickLog<T>(this T value, Color valueColor, string name)
        {
            if (DebugEnabled)
            {
                ValueDebugger.ValueLog(name, ToStringNull(value).QuickColor(valueColor));
            }

            if (LogToConsole)
            {
                QuickCLog(value, valueColor, name);
            }

            return value;
        }

        public static T QuickCLog<T>(this T value, string name, LogType logType = LogType.Log)
        {
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError($"{name} - {value}");
                    break;
                case LogType.Assert:
                    Debug.LogAssertion($"{name} - {value}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"{name} - {value}");
                    break;
                case LogType.Log:
                    Debug.Log($"{name} - {value}");
                    break;
                case LogType.Exception:
                    Debug.LogError($"{name} - {value}");
                    if (value is Exception e)
                    {
                        Debug.LogException(e);
                    }
                    break;
            }
            return value;
        }

        public static T QuickCLog<T>(this T value, string name, Color nameColor, LogType logType = LogType.Log)
        {
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError($"{name.QuickColor(nameColor)} - {value}");
                    break;
                case LogType.Assert:
                    Debug.LogAssertion($"{name.QuickColor(nameColor)} - {value}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"{name.QuickColor(nameColor)} - {value}");
                    break;
                case LogType.Log:
                    Debug.Log($"{name.QuickColor(nameColor)} - {value}");
                    break;
                case LogType.Exception:
                    Debug.LogError($"{name.QuickColor(nameColor)} - {value}");
                    if(value is Exception e)
                    {
                        Debug.LogException(e);
                    }
                    break;
            }
         
            return value;
        }

        public static T QuickCLog<T>(this T value, Color valueColor, string name, Color nameColor, LogType logType = LogType.Log)
        {
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError($"{name.QuickColor(nameColor)} - {ToStringNull(value).QuickColor(valueColor)}");
                    break;
                case LogType.Assert:
                    Debug.LogAssertion($"{name.QuickColor(nameColor)} - {ToStringNull(value).QuickColor(valueColor)}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"{name.QuickColor(nameColor)} - {ToStringNull(value).QuickColor(valueColor)}");
                    break;
                case LogType.Log:
                    Debug.Log($"{name.QuickColor(nameColor)} - {ToStringNull(value).QuickColor(valueColor)}");
                    break;
                case LogType.Exception:
                    Debug.LogError($"{name.QuickColor(nameColor)} - {ToStringNull(value).QuickColor(valueColor)}");
                    if (value is Exception e)
                    {
                        Debug.LogException(e);
                    }
                    break;
            }
            return value;
        }

        public static T QuickCLog<T>(this T value, Color valueColor, string name, LogType logType = LogType.Log)
        {
            Debug.Log($"{name} - {ToStringNull(value).QuickColor(valueColor)}");
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError($"{name} - {ToStringNull(value).QuickColor(valueColor)}");
                    break;
                case LogType.Assert:
                    Debug.LogAssertion($"{name} - {ToStringNull(value).QuickColor(valueColor)}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"{name} - {ToStringNull(value).QuickColor(valueColor)}");
                    break;
                case LogType.Log:
                    Debug.Log($"{name} - {ToStringNull(value).QuickColor(valueColor)}");
                    break;
                case LogType.Exception:
                    Debug.LogError($"{name} - {ToStringNull(value).QuickColor(valueColor)}");
                    if (value is Exception e)
                    {
                        Debug.LogException(e);
                    }
                    break;
            }
            return value;
        }

        public static string QuickColor(this string value, Color color)
        {
            const string colorFormat = "<color=#{0}>{1}</color>";
            return string.Format(colorFormat, ColorUtility.ToHtmlStringRGB(color), value);
        }

        private static string ToStringNull<T>(T value)
        {
            if (value == null)
            {
                if (value is string)
                    return QuickColor("null", Color.red);
                else
                    return default;
            }
            return value.ToString();
        }
    }
}