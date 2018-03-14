﻿using UnityEngine;

namespace Util
{
    public class _2DCamMove : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        [SerializeField]
        private bool useDeltaTime;

        private void Update()
        {
            transform.position += move();
        }

        private Vector3 move()
        {
            Vector3 output = new Vector3();
            output.x = Input.GetAxis("Horizontal") * speed * (useDeltaTime ? Time.deltaTime : 1);
            output.y = Input.GetAxis("Vertical") * speed * (useDeltaTime ? Time.deltaTime : 1);

            return output;
        }
    }
}