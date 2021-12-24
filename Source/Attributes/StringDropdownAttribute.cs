using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace NoUtil
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StringDropdownAttribute : PropertyAttribute
    {
        private string targetFieldName;
        private Type classType;

        public StringDropdownAttribute(string targetFieldName, Type classType)
        {
            this.targetFieldName = targetFieldName;
            this.classType = classType;
        }

        public IEnumerable<string> GetField()
        {
#if UNITY_EDITOR
            var assets = AssetDatabase.FindAssets($"t:{classType}");
            if (assets.Length > 0)
            {
                var asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), classType);
                var fieldInfo = classType.GetField(targetFieldName);
                var value = fieldInfo?.GetValue(asset) ?? default;
                return value is IEnumerable<string> values ? values : Array.Empty<string>();
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