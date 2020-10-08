using UnityEngine;
using UnityEditor;
using System;

namespace Util
{
    [Serializable, AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class LayerDropdownAttribute : PropertyAttribute
    {
        public override bool Match(object obj)
        {
            return obj is int;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LayerDropdownAttribute))]
    public class LayerDropdownDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (var p = new EditorGUI.PropertyScope(position, label, property))
            {
                if (property.type == "int")
                    property.intValue = EditorGUI.LayerField(position, label, property.intValue);
                else
                    EditorGUI.PropertyField(position, property, label);
            }
        }
    }
#endif
}
