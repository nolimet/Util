using UnityEngine;

namespace Util.Update
{
    public class BaseClass : MonoBehaviour, IUpdatable
    {
        // Use this for initialization
        private void Start()
        {
            UpdateManager.AddUpdateAble(this);
        }

        private void OnEnable()
        {
            UpdateManager.AddUpdateAble(this);
        }

        private void OnDisable()
        {
            UpdateManager.RemoveUpdateAble(this);
        }

        public virtual void IUpdate()
        {
            Debug.Log(System.DateTime.Now);
        }
    }
}