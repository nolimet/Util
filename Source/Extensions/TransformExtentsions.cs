using System.Linq;
using UnityEngine;

/// <summary>
/// Util libary
/// Contains a quite a few usefull things like a serialization module, A event system and some hacks for the UI system
/// </summary>
namespace NoUtil
{
    /// <summary>
    /// Common Utily libary. It contains fuctions that I used regulary or where very hard to figure out.
    /// </summary>
    public static class TransformExtentsions
    {
        /// <summary>
        /// Gets the bounds of a object including it's childeren
        /// </summary>
        /// <param name="obj">object's transform that contains the childeren</param>
        /// <returns>Bounds including Parents childeren</returns>
        public static Bounds GetChildBounds(this Transform obj)
        {
            Bounds bounds;
            // First find a center for your bounds.
            Vector3 center = Vector3.zero;
            foreach (Transform child in obj.transform)
            {
                center += child.gameObject.GetComponent<SpriteRenderer>().bounds.center;
            }
            center /= obj.transform.childCount; //center is average center of children

            //Now you have a center, calculate the bounds by creating a zero sized 'Bounds',
            bounds = new Bounds(center, Vector3.zero);

            foreach (Transform child in obj.transform)
            {
                bounds.Encapsulate(child.gameObject.GetComponent<SpriteRenderer>().bounds);
            }
            return bounds;
        }

        /// <summary>
        /// Gets the bounds of a object including it's childeren
        /// </summary>
        /// <param name="obj">object's transform that contains the childeren</param>
        /// <param name="ignorNameTag">Object names to ignor usefull to create mask</param>
        /// <returns>Bounds including Parents childeren</returns>
        public static Bounds GetChildBounds(this Transform obj, string ignorNameTag)
        {
            Bounds bounds;

            //so i don't do useless checks
            if (ignorNameTag == "")
            {
                // First find a center for your bounds.
                Vector3 center = Vector3.zero;
                foreach (Transform child in obj.transform)
                {
                    center += child.gameObject.GetComponent<SpriteRenderer>().bounds.center;
                }
                center /= obj.transform.childCount; //center is average center of children

                //Now you have a center, calculate the bounds by creating a zero sized 'Bounds',
                bounds = new Bounds(center, Vector3.zero);

                foreach (Transform child in obj.transform)
                {
                    bounds.Encapsulate(child.gameObject.GetComponent<SpriteRenderer>().bounds);
                }
            }
            //
            else
            {
                ignorNameTag = ignorNameTag.ToLower();
                // First find a center for your bounds.
                Vector3 center = Vector3.zero;
                int i = 0;
                foreach (Transform child in obj.transform)
                {
                    if (!child.gameObject.name.ToLower().Contains(ignorNameTag))
                    {
                        //Debug.Log(child.name);
                        center += child.gameObject.GetComponent<SpriteRenderer>().bounds.center;
                        i++;
                    }
                }
                center /= i; //center is average center of children

                //Now you have a center, calculate the bounds by creating a zero sized 'Bounds',
                bounds = new Bounds(center, Vector3.zero);

                foreach (Transform child in obj.transform)
                {
                    if (!child.name.ToLower().Contains(ignorNameTag))
                        bounds.Encapsulate(child.gameObject.GetComponent<SpriteRenderer>().bounds);
                }
            }
            return bounds;
        }

        /// <summary>
        /// Gets the bounds of a object including it's childeren
        /// </summary>
        /// <param name="obj">object's transform that contains the childeren</param>
        /// <param name="IgnorNameTags">A set of name part to ignor used to create more complex masks</param>
        /// <returns>Bounds including Parents childeren</returns>
        public static Bounds GetChildBounds(this Transform obj, string[] IgnorNameTags)
        {
            if (IgnorNameTags.Length == 0)
                return obj.GetChildBounds();

            Bounds bounds;
            Vector3 center = Vector3.zero;
            int i = 0;
            string n;

            for (int j = 0; j < IgnorNameTags.Length; j++)
            {
                IgnorNameTags[j] = IgnorNameTags[j].ToLower();
            }

            // First find a center for your bounds.s
            foreach (Transform child in obj.transform)
            {
                n = child.gameObject.name.ToLower();
                if (IgnorNameTags.Any(str => n.Contains(str)))
                {
                    //Debug.Log(child.name);
                    center += child.gameObject.GetComponent<SpriteRenderer>().bounds.center;
                    i++;
                }
            }
            center /= i; //center is average center of children

            //Now you have a center, calculate the bounds by creating a zero sized 'Bounds',
            bounds = new Bounds(center, Vector3.zero);

            foreach (Transform child in obj.transform)
            {
                n = child.gameObject.name.ToLower();
                if (IgnorNameTags.Any(str => n.Contains(str)))
                    bounds.Encapsulate(child.gameObject.GetComponent<SpriteRenderer>().bounds);
            }

            return bounds;
        }

        /// <summary>
        /// Input array to return combined bounds of array
        /// </summary>
        /// <param name="objs">objects to get combined bound of</param>
        /// <returns>Combined bound</returns>
        public static Bounds GetBounds(this Transform[] objs)
        {
            Bounds bounds;
            // First find a center for your bounds.
            Vector3 center = Vector3.zero;
            foreach (Transform child in objs)
            {
                center += child.gameObject.GetComponent<SpriteRenderer>().bounds.center;
            }
            center /= objs.Length; //center is average center of children

            //Now you have a center, calculate the bounds by creating a zero sized 'Bounds',
            bounds = new Bounds(center, Vector3.zero);

            foreach (Transform child in objs)
            {
                bounds.Encapsulate(child.gameObject.GetComponent<SpriteRenderer>().bounds);
            }
            return bounds;
        }

        public static Bounds GetBounds(this Transform obj)
        {
            return obj.gameObject.GetComponent<SpriteRenderer>().bounds;
        }

        /// <summary>
        /// Draw the bounds of a object
        /// </summary>
        /// <param name="b">Bounds that will be drawn</param>
        public static void DrawBounds(Bounds b)
        {
            Debug.DrawLine(b.max, new Vector3(b.max.x, b.min.y));
            Debug.DrawLine(new Vector3(b.max.x, b.min.y), b.min);
            Debug.DrawLine(b.min, new Vector3(b.min.x, b.max.y));
            Debug.DrawLine(new Vector3(b.min.x, b.max.y), b.max);
            Debug.DrawLine(b.max, b.min, Color.red);
            Debug.DrawLine(new Vector3(b.min.x, b.max.y), new Vector3(b.max.x, b.min.y), Color.red);
        }

        /// <summary>
        /// Check if bounds are inside the target
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="target">Object contained in other bound</param>
        /// <returns></returns>
        public static bool ContainBounds(this Bounds bounds, Bounds target)
        {
            return bounds.Contains(target.min) && bounds.Contains(target.max);
        }
    }
}