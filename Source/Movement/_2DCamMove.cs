using UnityEngine;

namespace NoUtil.Movement
{
    public class _2DCamMove : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        [SerializeField]
        private bool useDeltaTime;

        private void Update()
        {
            transform.position += GetMoveVector();
        }

        private Vector3 GetMoveVector()
        {
            Vector3 output = new Vector3
            {
                x = Input.GetAxis("Horizontal") * speed * (useDeltaTime ? Time.deltaTime : 1),
                y = Input.GetAxis("Vertical") * speed * (useDeltaTime ? Time.deltaTime : 1)
            };

            return output;
        }
    }
}