//Auther Jesse D. Stam
//License - MIT License
//Source 

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Util.Debugger
{
    /// <summary>
    /// On screen debugger call class
    /// </summary>
    public static class Debugger
    {
        public static bool DebugEnabled = true;
        public static bool LogToConsole = false;

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
                ValueDebugger.ValueLog(name.QuickColor(nameColor), value.ToString().QuickColor(valueColor));
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
                ValueDebugger.ValueLog(name, value);
            }

            if (LogToConsole)
            {
                QuickCLog(value, valueColor, name);
            }

            return value;
        }

        public static T QuickCLog<T>(this T value, string name)
        {
            Debug.Log($"{name} - {value}");
            return value;
        }

        public static T QuickCLog<T>(this T value, string name, Color nameColor)
        {
            Debug.Log($"{name.QuickColor(nameColor)} - {value}");
            return value;
        }

        public static T QuickCLog<T>(this T value, Color valueColor, string name, Color nameColor)
        {
            Debug.Log($"{name.QuickColor(nameColor)} - {value.ToString().QuickColor(valueColor)}");
            return value;
        }

        public static T QuickCLog<T>(this T value, Color valueColor, string name)
        {
            Debug.Log($"{name} - {value.ToString().QuickColor(valueColor)}");
            return value;
        }

        public static string QuickColor(this string value, Color color)
        {
            const string colorFormat = "<color=#{0}>{1}</color>";
            return string.Format(colorFormat, ColorUtility.ToHtmlStringRGB(color), value);
        }
    }

    /// <summary>
    /// On screen debugger usefull when working with a game build but you want to do some error tracking
    /// </summary>
    public class ValueDebugger : MonoBehaviour
    {
        protected Dictionary<string, object> Values;
        protected Text t;
        private static ValueDebugger instance;
        private string ts;

        /// <summary>
        /// Value that will be logged. Also create the object that will be rendered onscreen
        /// completely by code so it does not need a prefab
        /// </summary>
        /// <param name="name">Value's name so you can find it back</param>
        /// <param name="value">Value of the object</param>
        public static void ValueLog(string name, object value)
        {
            if (instance == null)
            {
                // if it just lost it's instance
                instance = FindObjectOfType<ValueDebugger>();
            }

            if (instance == null)
            {
                ///Make object if it does not exist
                //Canvas
                GameObject g = new GameObject();

                Canvas c = g.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                c.sortingOrder = 7000;

                CanvasScaler sc = g.AddComponent<CanvasScaler>();
                sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                sc.referenceResolution = new Vector2(1600, 900);
                sc.matchWidthOrHeight = 1;

                //text Display
                GameObject g2 = new GameObject();
                g2.transform.SetParent(g.transform, false);

                RectTransform rt = g2.AddComponent<RectTransform>();
                rt.anchorMax = new Vector2(1f, 1f);
                rt.anchorMin = new Vector2(0.5f, 0);
                rt.sizeDelta = new Vector2(-20, -40);

                rt.anchoredPosition = new Vector2(-20, 0);

                Text t = g2.AddComponent<Text>();
                t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                t.color = Color.green;
                t.fontSize = 20;
                g2.AddComponent<ValueDebugger>();

                //background Image
                GameObject g3 = new GameObject();
                g3.transform.SetParent(g.transform, false);
                g3.transform.SetAsFirstSibling();

                rt = g3.AddComponent<RectTransform>();
                rt.anchorMax = new Vector2(1f, 1f);
                rt.anchorMin = new Vector2(0.5f, 0);
                rt.sizeDelta = new Vector2(-20, -40);

                rt.anchoredPosition = new Vector2(-20, 0);

                Image I = g3.AddComponent<Image>();
                I.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);

                g.name = "util.DebugVisual";
            }

            if (instance && !instance.transform.parent.gameObject.activeSelf)
            {
                instance.transform.parent.gameObject.SetActive(true);
            }

            if (instance.Values.Keys.Contains(name))
            {
                instance.Values[name] = value;
            }
            else
            {
                instance.Values.Add(name, value);
            }
        }

        private void Awake()
        {
            instance = this;
            t = GetComponent<Text>();
            Values = new Dictionary<string, object>();
        }

        private void Update()
        {
            if (!Debugger.DebugEnabled)
            {
                transform.parent.gameObject.SetActive(false);
            }

            Process();
        }

        private void OnDestroy()
        {
            instance = null;
        }

        /// <summary>
        /// create the string that will be displayed on screen
        /// </summary>
        private void Process()
        {
            ts = "";

            foreach (KeyValuePair<string, object> vs in Values)
            {
                ts += vs.Key + " : " + vs.Value.ToString() + " \n";
            }

            t.text = ts;
        }
    }
}
