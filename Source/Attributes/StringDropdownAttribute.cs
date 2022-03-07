using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace NoUtil
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StringDropdownAttribute : PropertyAttribute
    {
        private static readonly BindingFlags bindingFlagsReflection = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly BindingFlags bindingFlagFunctionLookup = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Public;

        public delegate IEnumerable<string> StringDropdownFunc(IEnumerable<object> objects);

        private string targetFieldName;
        private Type classType;
        private string staticFunctionName;
        private StringDropdownFunc func;

        public StringDropdownAttribute(string targetFieldName, Type classType)
        {
            this.targetFieldName = targetFieldName;
            this.classType = classType;
        }

        /// <summary>
        /// Get value using a get Function
        /// </summary>
        /// <param name="classType">class type where you want ot get the value from</param>
        /// <param name="staticFunctionName">Name of the static function used to get the value</param>
        public StringDropdownAttribute(Type classType, string staticFunctionName)
        {
            this.classType = classType;
            this.staticFunctionName = staticFunctionName;

            var miHandler = classType.GetMethod(staticFunctionName, bindingFlagFunctionLookup);
            func = (StringDropdownFunc)Delegate.CreateDelegate(typeof(StringDropdownFunc), miHandler);
        }

        public IEnumerable<string> GetField()
        {
#if UNITY_EDITOR
            var assets = AssetDatabase.FindAssets($"t:{classType}");

            if (assets.Length > 0)
            {
                if (func != null)
                {
                    var loadedAssets = assets.Select(x => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(x), classType));
                    return func(loadedAssets);
                }
                else
                {
                    var splitField = targetFieldName.Split('.');

                    var asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), classType);
                    var fieldInfo = classType.GetField(splitField[0], bindingFlagsReflection);
                    var value = fieldInfo?.GetValue(asset) ?? default;

                    if (splitField.Length == 2)
                    {
                        var arr = value as Array;
                        List<string> list = new List<string>();
                        foreach (var item in arr)
                        {
                            //Split across lines to help with debugging
                            var type = item.GetType();
                            var field = type.GetField(splitField[1], bindingFlagsReflection);
                            var val = field.GetValue(item) ?? default;

                            if (val is string str)
                            {
                                list.Add(str);
                            }
                        }

                        return list;
                    }
                    return value is IEnumerable<string> values ? values : Array.Empty<string>();
                }
            }
#endif
            return Array.Empty<string>();
        }

#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(StringDropdownAttribute))]
        public class StringDropdownAttributeAttributeDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (property.type == "string")
                {
                    var att = attribute as StringDropdownAttribute;
                    var displayOptions = att.GetField().Select(x => new GUIContent(x)).ToList();

                    int selectedIndex = displayOptions.FindIndex(x => x.text == property.stringValue);
                    int newIndex = EditorGUI.Popup(position, label, selectedIndex, displayOptions.ToArray());

                    if (newIndex >= 0)
                    {
                        property.stringValue = displayOptions[newIndex].text;
                    }
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
        }

#endif
    }
}