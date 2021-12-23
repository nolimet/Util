using UnityEditor;

namespace NoUtil.Editor
{
    /// <summary>
    /// Forces a reset on Edetor Camera FOV
    /// </summary>
    public static class ResetCameraFOV
    {
        [MenuItem("NoUtil/Camera/ResetNearClipPlane")]
        private static void ResetNearClipPlane()
        {
            //SceneView.lastActiveSceneView.camera.fieldOfView = 0.1f;
            SceneView.lastActiveSceneView.camera.nearClipPlane = 0.005f;

            SceneView.lastActiveSceneView.Repaint();
        }
    }
}