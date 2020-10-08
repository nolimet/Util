//Author Jesse D. Stam
//License - MIT License
//Source https://github.com/nolimet/Util/blob/master/Debug/ValueDebugger.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Jesse.Utility.Debugging
{
    /// <summary>
    /// On screen debugger usefull when working with a game build but you want to do some value tracking
    /// </summary>
    public class ValueDebugger : MonoBehaviour
    {
        private static ValueDebugger instance;
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        private Text textField;
        private StringBuilder displayBuilder;

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

            //If instance == null create a new window without using a prefab
            if (instance == null)
            {
                ///Make object if it does not exist
                //Canvas
                GameObject canvasGameObject = new GameObject();

                Canvas canvas = canvasGameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 30000;

                CanvasScaler canvasScaler = canvasGameObject.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1600, 900);
                canvasScaler.matchWidthOrHeight = 1;

                //text Display
                GameObject gameObjectText = new GameObject();
                gameObjectText.transform.SetParent(canvasGameObject.transform, false);

                RectTransform rectTransform = gameObjectText.AddComponent<RectTransform>();
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.sizeDelta = new Vector2(-20, -40);

                rectTransform.anchoredPosition = new Vector2(-20, 0);

                Text textfield = gameObjectText.AddComponent<Text>();
                textfield.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                textfield.color = Color.green;
                textfield.fontSize = 20;
                instance = gameObjectText.AddComponent<ValueDebugger>();

                //background Image
                GameObject backgroundGameOBject = new GameObject();
                backgroundGameOBject.transform.SetParent(canvasGameObject.transform, false);
                backgroundGameOBject.transform.SetAsFirstSibling();

                rectTransform = backgroundGameOBject.AddComponent<RectTransform>();
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.sizeDelta = new Vector2(-20, -40);

                rectTransform.anchoredPosition = new Vector2(-20, 0);

                Image background = backgroundGameOBject.AddComponent<Image>();
                background.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);

                canvasGameObject.name = "util.DebugVisual";
            }

            if (instance && !instance.transform.parent.gameObject.activeSelf)
            {
                instance.transform.parent.gameObject.SetActive(true);
            }

            if (instance.values.Keys.Contains(name))
            {
                instance.values[name] = value;
            }
            else
            {
                instance.values.Add(name, value);
            }
        }

        private void Awake()
        {
            instance = this;
            textField = GetComponent<Text>();

            displayBuilder = new StringBuilder();

            Debugger.DebugEnabledChanged += OnDebuggerEnableChanged;
            Debugger.ClearedDisplay += OnClearedDisplay;
        }

        private void OnClearedDisplay()
        {
            values.Clear();
        }

        private void OnDebuggerEnableChanged(bool isEnabled)
        {
            transform.parent.gameObject.SetActive(isEnabled);
        }

        private void Update()
        {
            Process();
        }

        private void OnDestroy()
        {
            instance = null;
            Debugger.DebugEnabledChanged -= OnDebuggerEnableChanged;
            Debugger.ClearedDisplay -= OnClearedDisplay;

        }

        /// <summary>
        /// create the string that will be displayed on screen
        /// </summary>
        private void Process()
        {
            displayBuilder.Clear();

            foreach (KeyValuePair<string, object> vs in values)
            {
                displayBuilder.AppendLine(vs.Key + " : " + (vs.Value?.ToString() ?? "null")); //checks value to see  if it's null because if it is it can cause the display to 'crash'
            }

            textField.text = displayBuilder.ToString();
        }
    }
}