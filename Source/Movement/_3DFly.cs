using UnityEngine;

namespace NoUtil.Movement
{
    [AddComponentMenu("Camera-Control/3DFly")]
    [RequireComponent(typeof(MouseLook))]
    public class _3DFly : MonoBehaviour
    {
        private Rigidbody rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");
            if (hor != 0 || ver != 0)
            {
                float sprint = Input.GetAxis("Sprint");
                if (rigidbody && GetComponent<Rigidbody>() != null)
                {
                    const float sprintMultiplier = 2f;
                    const float normalForce = 60f;

                    rigidbody.AddRelativeForce((sprint > 0 ? sprintMultiplier : 1) * normalForce * Time.deltaTime * new Vector3(hor, 0, ver));
                }
                else
                {
                    const float sprintMultiplier = 2f;
                    const float normalSpeed = 6f;

                    transform.Translate((sprint > 0 ? sprintMultiplier : 1) * normalSpeed * Time.deltaTime * new Vector3(hor, 0, ver));
                }
            }
            else if (rigidbody)
            {
                rigidbody.velocity = Vector3.zero;
            }
        }
    }
}