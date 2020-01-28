using UnityEngine;
using UnityEditor;

public class CreateMenuExtensions : ScriptableObject
{
    [MenuItem("GameObject/ToggleActive",false,-99999)]
    static void ToggleActive()
    {
        foreach(var t in Selection.transforms)
        {
            t.gameObject.SetActive(!t.gameObject.activeSelf);
        }
    }
}
