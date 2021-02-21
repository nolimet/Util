using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoUtil.Extentsions
{
    public static class GameObjectExtentsions
    {
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            T returnValue = component.GetComponent<T>();
            if (!returnValue)
            {
                returnValue = component.gameObject.AddComponent<T>();
            }
            return returnValue;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T returnValue = gameObject.GetComponent<T>();
            if (!returnValue)
            {
                returnValue = gameObject.AddComponent<T>();
            }
            return returnValue;
        }
    }
}