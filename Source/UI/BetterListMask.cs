using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NoUtil.UI
{
    public class BetterListMask : MonoBehaviour
    {
        private static readonly Vector3[] worldCorners = new Vector3[4];

        [SerializeField]
        private RectTransform viewPort;

        [SerializeField, Tooltip("The points are from the center")]
        private Vector2[] checkingPoints = new Vector2[0];

        [SerializeField, ReadOnly]
        private List<ListItem> listItems = new List<ListItem>();

        private void Update()
        {
            //when the number of children does not match or when any item is invalid then remove the broken children and add the missing ones
            if (listItems.Count != transform.childCount || listItems.Any(x => !x.itemTransform || x.itemTransform.parent != transform))
            {
                //finding the broken ones(destroyed or moved)
                var removedItems = listItems.Where(x => !x.itemTransform || x.itemTransform.parent != transform).ToArray();
                foreach (var removedItem in removedItems)
                {
                    listItems.Remove(removedItem);
                }

                //Find new children and adding them to the list
                foreach (Transform child in transform)
                {
                    if (listItems.All(x => x.itemTransform != child))
                    {
                        listItems.Add(new ListItem(child as RectTransform));
                    }
                }
            }

            var worldRect = GetWorldRect(viewPort);
            foreach (var listIem in listItems)
            {
                var listWorldRect = GetWorldRect(listIem.itemTransform);

                Vector2 center = listWorldRect.center;
                listIem.SetGraphicsEnabled
                    (
                        checkingPoints.Any(point => worldRect.Contains(new Vector2
                            (
                                x: center.x + (listWorldRect.width / 2) * point.x,
                                y: center.y + (listWorldRect.height / 2) * point.y
                            )))
                    );
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;

            //draw the items
            foreach (var item in listItems)
            {
                item.DrawWorldRect(checkingPoints);
            }

            //draw the viewport
            var worldRect = GetWorldRect(viewPort);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(worldCorners[0], worldCorners[1]);
            Gizmos.DrawLine(worldCorners[1], worldCorners[2]);
            Gizmos.DrawLine(worldCorners[2], worldCorners[3]);
            Gizmos.DrawLine(worldCorners[3], worldCorners[0]);
        }

        private Rect GetWorldRect(RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners(worldCorners);
            var worldRect = new Rect
            {
                xMin = worldCorners[0].x,
                yMin = worldCorners[0].y,
                xMax = worldCorners[2].x,
                yMax = worldCorners[2].y
            };

            return worldRect;
        }

        [System.Serializable]
        private class ListItem
        {
            public readonly RectTransform itemTransform;

            [SerializeField, ReadOnly]
            private Graphic[] graphics;

            public ListItem(RectTransform itemTransform)
            {
                this.itemTransform = itemTransform;
                this.graphics = GetAllGraphicsFromTransform(itemTransform);

                //Find all graphics on this object.
                Graphic[] GetAllGraphicsFromTransform(Transform transform)
                {
                    List<Graphic> graphics = new List<Graphic>();
                    List<Transform> visited = new List<Transform>();
                    foreach (Transform child in transform)
                    {
                        GetGraphicsFromChildren(child);
                    }

                    return graphics.ToArray();
                    void GetGraphicsFromChildren(Transform child)
                    {
                        visited.Add(child);
                        graphics.AddRange(child.GetComponents<Graphic>());
                        foreach (Transform childsChild in transform)
                        {
                            if (!visited.Contains(child))
                            {
                                GetGraphicsFromChildren(childsChild);
                            }
                        }
                    }
                }
            }

            public void SetGraphicsEnabled(bool isActive)
            {
                foreach (var graphic in graphics)
                {
                    graphic.enabled = isActive;
                }
            }

            /// <summary>
            /// Debug fluf to help when things don't seem to work as intended.
            /// </summary>
            /// <param name="checkingPoints">The points where the actual checks are done</param>
            public void DrawWorldRect(Vector2[] checkingPoints)
            {
                itemTransform.GetWorldCorners(worldCorners);
                var worldRect = new Rect
                {
                    xMin = worldCorners[0].x,
                    yMin = worldCorners[0].y,
                    xMax = worldCorners[2].x,
                    yMax = worldCorners[2].y
                };

                //Draw the corners
                Gizmos.color = Color.red;
                for (int i = 0; i < 4; i++)
                {
                    Gizmos.DrawCube(worldCorners[i], Vector3.one);
                }

                //Draw a outline border
                Gizmos.DrawLine(worldCorners[0], worldCorners[1]);
                Gizmos.DrawLine(worldCorners[1], worldCorners[2]);
                Gizmos.DrawLine(worldCorners[2], worldCorners[3]);
                Gizmos.DrawLine(worldCorners[3], worldCorners[0]);

                //Draw the actual points
                Gizmos.color = Color.blue;
                Vector2 center = worldRect.center;
                foreach (var point in checkingPoints)
                {
                    Gizmos.DrawCube(new Vector2
                    (
                        x: center.x + (worldRect.width / 2) * point.x,
                        y: center.y + (worldRect.height / 2) * point.y
                    ), Vector3.one);
                }
            }
        }
    }
}