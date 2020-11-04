﻿using UnityEngine;

namespace NoUtil
{
    /// <summary>
    /// Scales with screen dimensions
    /// </summary>
    public class ScaleToScreenSize : MonoBehaviour
    {
        [SerializeField]
        private new Camera camera = null;

        public Vector3 screenSize = Vector3.zero;

        private void Awake()
        {
            Debug.Log("setting screen size");
            Vector3 p1 = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

            transform.localScale = new Vector3(p1.x, p1.y, 1) * 2f;
            screenSize = new Vector3(p1.x, p1.y, 1) * 2f;
        }
    }
}