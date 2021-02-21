using NoUtil.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NoUtil.Editor.UI
{
    [CustomEditor(typeof(UILayoutController))]
    public class UILayoutControllerEditor : UnityEditor.Editor
    {
        private const string layoutGroupsName = "layoutGroups";
        private const string alphaActiveName = "alphaActive";
        private const string alphaInactiveName = "alphaInactive";
        private const string defaultLayoutName = "defaultLayout";
        private const string layoutRootName = "layoutRoot";
        private const string changeDurationName = "changeDuration";

        private SerializedProperty layoutGroups;
        private SerializedProperty alphaActive;
        private SerializedProperty alphaInactive;
        private SerializedProperty defaultLayout;

        private GUIContent defaultLayoutGUIContent;
        private GUIContent alphaActiveGUIContent;
        private GUIContent alphaInactiveGUIContent;

        private ReorderableList layoutGroupsList;

        private void OnEnable()
        {
            layoutGroups = serializedObject.FindProperty(layoutGroupsName);
            alphaActive = serializedObject.FindProperty(alphaActiveName);
            alphaInactive = serializedObject.FindProperty(alphaInactiveName);
            defaultLayout = serializedObject.FindProperty(defaultLayoutName);

            layoutGroupsList = new ReorderableList(serializedObject, layoutGroups, true, true, true, true);

            layoutGroupsList.drawElementCallback += OnDrawElement;
            layoutGroupsList.elementHeightCallback += OnGetElementHeight;
            layoutGroupsList.onAddCallback += OnElementAdd;

            defaultLayoutGUIContent = new GUIContent(defaultLayout.displayName, defaultLayout.tooltip);
            alphaActiveGUIContent = new GUIContent("Active", alphaActive.tooltip);
            alphaInactiveGUIContent = new GUIContent("Inactive", alphaInactive.tooltip);
        }

        private void OnElementAdd(ReorderableList list)
        {
            int length = list.serializedProperty.arraySize++;

            SerializedProperty layoutGroup = layoutGroups.GetArrayElementAtIndex(length);
            SerializedProperty layoutRoot = layoutGroup.FindPropertyRelative(layoutRootName);
            SerializedProperty changeDuration = layoutGroup.FindPropertyRelative(changeDurationName);

            layoutRoot.objectReferenceValue = null;
            changeDuration.floatValue = 0.3f;
        }

        private float OnGetElementHeight(int index)
        {
            return 29;
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty layoutGroup = layoutGroups.GetArrayElementAtIndex(index);

            SerializedProperty layoutRoot = layoutGroup.FindPropertyRelative(layoutRootName);
            SerializedProperty changeDuration = layoutGroup.FindPropertyRelative(changeDurationName);

            rect.height = 21;
            rect.y += 4;

            Rect changeDurationRect = new Rect(rect);
            changeDurationRect.x += 5f;
            changeDurationRect.width = 30;

            Rect layoutRootRect = new Rect(rect);
            layoutRootRect.width -= 30 + 5 + 5;
            layoutRootRect.x += 30 + 5 + 5;

            EditorGUI.PropertyField(changeDurationRect, changeDuration, GUIContent.none);
            EditorGUI.PropertyField(layoutRootRect, layoutRoot, GUIContent.none);
        }

        private void DrawDefaultLayoutSelector()
        {
            int length = layoutGroups.arraySize;
            string[] names = new string[length + 2];

            string currentValue = defaultLayout.stringValue;
            names[0] = string.IsNullOrWhiteSpace(currentValue) ? "<no default object>" : $"[{currentValue}]";
            names[1] = "";

            for (int i = 0; i < length; i++)
            {
                //get the layoutRootObject reference so we can extract the name from it
                var obj = layoutGroups.GetArrayElementAtIndex(i).FindPropertyRelative(layoutRootName)?.objectReferenceValue;
                names[i + 2] = obj ? obj.name : string.Empty;
            }

            int selectedValue = 0;
            selectedValue = EditorGUILayout.Popup(defaultLayoutGUIContent, selectedValue, names);

            if (selectedValue > 1)
            {
                currentValue = names[selectedValue];
            }

            defaultLayout.stringValue = currentValue;
        }

        private void ValidateValues()
        {
            //Clamping the alpha values to 0-1 as high values don't work
            alphaActive.floatValue = Mathf.Clamp01(alphaActive.floatValue);
            alphaInactive.floatValue = Mathf.Clamp01(alphaInactive.floatValue);

            List<string> usedNames = new List<string>();
            for (int i = 0; i < layoutGroups.arraySize; i++)
            {
                var layoutGroup = layoutGroups.GetArrayElementAtIndex(i);
                var layoutRoot = layoutGroup.FindPropertyRelative(layoutRootName);
                var changeDuration = layoutGroup.FindPropertyRelative(changeDurationName);

                changeDuration.floatValue = Mathf.Max(0, changeDuration.floatValue); //making sure there is no negative duration

                var layoutRootRefrenceValue = layoutRoot.objectReferenceValue;
                usedNames.Add(layoutRootRefrenceValue ? layoutRootRefrenceValue.name : string.Empty);
            }

            var groupedNames = usedNames.GroupBy(x => x);
            var duplicateNames = groupedNames.Where(x => x.Count() > 1 && x.Key != string.Empty);

            if (groupedNames.Any(x => x.Key == string.Empty))
            {
                EditorGUILayout.HelpBox("There are empty names", MessageType.Warning, true);
            }

            if (duplicateNames.Count() > 0)
            {
                EditorGUILayout.HelpBox($"Duplicate names!\n {string.Join("\n", duplicateNames.Select(x => x.Key))}", MessageType.Error, true);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                DrawDefaultLayoutSelector();
                using (var v1 = new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("Alpha");
                    using (var h1 = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(alphaActive, alphaActiveGUIContent);
                        EditorGUILayout.PropertyField(alphaInactive, alphaInactiveGUIContent);
                    }
                }
                EditorGUILayout.Space();
                layoutGroupsList.DoLayoutList();
                ValidateValues();
                if (changeScope.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}