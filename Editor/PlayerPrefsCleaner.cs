using UnityEditor;
using UnityEngine;

public class PlayerPrefsCleaner : MonoBehaviour
{
    [MenuItem("Util/Clear PlayerPrefs")]
    static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs Cleared");
    }

}