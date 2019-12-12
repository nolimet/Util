using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Util.UI
{
    public class DragableObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Header("Events")]
        public UnityEvent DragStarted;

        public UnityEvent DragEnded;

        [Header("Required"), SerializeField] private RectTransform movingGraphic;
        [SerializeField] private RectTransform elementSelfContainer;

        [Header("Required before first use"), SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform viewport;

        [Header("Settings"), SerializeField, Tooltip("Allows it to tell the layout to update")] private AnimatedVerticalLayout verticalLayout;
        [SerializeField, Tooltip("The axis that stays the same")] private LockedAxis lockedAxis = LockedAxis.Horizontal;
        [SerializeField, Tooltip("As you drag the element around should it assume the location of other elements")] private bool reorderHierarchy = true;

        [SerializeField, Tooltip("Should we reset the position to where we started in the original parent? \nCould be used for when you have a visual and element that moves around in the hierarchy ")]
        private bool resetPositionAfterDrag = true;

        [SerializeField, Tooltip("For when the dragable element is inside a scrollrect. So it can scroll up, down, left or right")] private bool manipulateParentScrollrect = false;
        [SerializeField, Tooltip("Scroll speed is in pixels per second")] private float scrollSpeed = 100f;

        private RectTransform dragContainer;
        private Transform originalParent;
        private Vector2 mouseOffset;
        private Vector2 originalAnchoredPosition;
        private RectTransform elementSelfParent;
        private RectTransform lastOverlap;
        private ScrollRect scrollRect;
        private EventSystem eventSystem;

        private bool dragging;

        //Updated while draging
        private Vector3 lastUnclampedPosition = Vector3.zero;

        private Vector3[] cornersGraphic = new Vector3[4];
        private Vector3[] cornersViewport = new Vector3[4];

        public void Setup(Canvas canvas, AnimatedVerticalLayout verticalLayout, RectTransform viewport)
        {
            dragContainer = canvas.transform as RectTransform;
            this.canvas = canvas;
            this.verticalLayout = verticalLayout;
            this.viewport = viewport;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            scrollRect = GetComponentInParent<ScrollRect>();
            mouseOffset = eventData.position - (Vector2)movingGraphic.position;

            movingGraphic.SetParent(dragContainer);
            movingGraphic.SetAsLastSibling();

            elementSelfParent = elementSelfContainer.parent as RectTransform;
            dragging = true;

            DragStarted?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            const float raycastPosition = 0.45f;//45% of height/2 is added to the center point location of the graphic while dragging

            eventSystem = EventSystem.current;
            PointerEventData p = new PointerEventData(eventSystem);
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            (Rect graphic, Rect viewport) rects = CreateRects();

            //Updating position of drag-able graphic
            Vector3 originalPosition = movingGraphic.position;
            Vector3 unclampedPosition = movingGraphic.position;
            switch (lockedAxis)
            {
                case LockedAxis.Horizontal:
                    unclampedPosition.y = eventData.position.y - mouseOffset.y;
                    break;

                case LockedAxis.Vertical:
                    unclampedPosition.x = eventData.position.x - mouseOffset.x;
                    break;

                case LockedAxis.None:
                    unclampedPosition = eventData.position - mouseOffset;
                    break;
            }
            lastUnclampedPosition = unclampedPosition;
            Vector3 clampedPosition = ClampPosition(unclampedPosition, rects.graphic, rects.viewport);
            movingGraphic.position = clampedPosition;
            if (reorderHierarchy)
            {
                //Starting check to see if there is any overlap with any graphics
                bool hadOverlap = false;
                if (lockedAxis != LockedAxis.Vertical)
                {
                    //Checking to see if something above the dragableGrahpic if so then there is overlap
                    hadOverlap = !hadOverlap && originalPosition.y < unclampedPosition.y ? CheckForOverlap((new Vector2(0, movingGraphic.pivot.y * raycastPosition) * canvas.scaleFactor) + (Vector2)unclampedPosition) : hadOverlap;
                    //Checking to see if something is below the dragableGraphic if so then there is overlap
                    hadOverlap = !hadOverlap && originalPosition.y > unclampedPosition.y ? CheckForOverlap((new Vector2(0, (1 - movingGraphic.pivot.y) * raycastPosition) * canvas.scaleFactor) + (Vector2)unclampedPosition) : hadOverlap;
                }
                if (lockedAxis != LockedAxis.Horizontal)
                {
                    hadOverlap = !hadOverlap && originalPosition.x < unclampedPosition.x ? CheckForOverlap((new Vector2(movingGraphic.pivot.x * raycastPosition, 0) * canvas.scaleFactor) + (Vector2)unclampedPosition) : hadOverlap;
                    //Checking to see if something is below the dragableGraphic if so then there is overlap
                    hadOverlap = !hadOverlap && originalPosition.x > unclampedPosition.x ? CheckForOverlap((new Vector2((1 - movingGraphic.pivot.x) * raycastPosition, 0) * canvas.scaleFactor) + (Vector2)unclampedPosition) : hadOverlap;
                }

                if (hadOverlap)
                {
                    //If we had overlap then update the layout as the transforms changed
                    if (verticalLayout)
                    {
                        verticalLayout.UpdatePositionAndCollectChildren();
                    }
                }
                else
                {
                    //If where not then clear lastOverlap
                    lastOverlap = null;
                }
            }

            bool CheckForOverlap(Vector2 screenCheckPosition)
            {
                p.position = screenCheckPosition;
                eventSystem.RaycastAll(p, raycastResults);

                if (raycastResults.Count > 0)
                {
                    int elementIndex = raycastResults.FindIndex(e =>
                    {
                        Transform element = e.gameObject.transform;
                        return element.parent == elementSelfParent && element != movingGraphic && element != elementSelfContainer;
                    });

                    if (elementIndex >= 0)
                    {
                        RectTransform element = raycastResults[elementIndex].gameObject.transform as RectTransform;
                        if (element != lastOverlap)
                        {
                            elementSelfContainer.SetSiblingIndex(element.GetSiblingIndex());

                            lastOverlap = element;
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            movingGraphic.SetParent(originalParent);
            if (resetPositionAfterDrag)
            {
                movingGraphic.anchoredPosition = originalAnchoredPosition;
            }

            dragging = false;
            DragEnded?.Invoke();
            if (verticalLayout)
            {
                verticalLayout.UpdateTargetPositions();
            }
            if (manipulateParentScrollrect && scrollRect)
            {
                scrollRect.velocity = Vector2.zero;
            }
        }

        private (Rect graphic, Rect viewport) CreateRects()
        {
            movingGraphic.GetWorldCorners(cornersGraphic);
            this.viewport.GetWorldCorners(cornersViewport);

            Rect graphic = new Rect
            {
                xMin = cornersGraphic.Min(x => x.x),
                yMin = cornersGraphic.Min(x => x.y),
                xMax = cornersGraphic.Max(x => x.x),
                yMax = cornersGraphic.Max(x => x.y)
            };

            Rect viewport = new Rect()
            {
                xMin = cornersViewport.Min(x => x.x),
                yMin = cornersViewport.Min(x => x.y),
                xMax = cornersViewport.Max(x => x.x),
                yMax = cornersViewport.Max(x => x.y)
            };

            return (graphic, viewport);
        }

        private Vector3 ClampPosition(Vector3 newPosition, Rect graphic, Rect viewport)
        {
            //if (!elementSelfParent.Contains(graphic.min) || !elementSelfParent.Contains(graphic.max))
            //{
            Vector2 alteredPos = new Vector2
            (
                x: Mathf.Clamp
                    (
                        value: newPosition.x,
                        min: viewport.xMin + ((1 - movingGraphic.pivot.x) * graphic.width),
                        max: viewport.xMax - (movingGraphic.pivot.x * graphic.width)
                    ),
                y: Mathf.Clamp
                    (
                        value: newPosition.y,
                        min: viewport.yMin + ((1 - movingGraphic.pivot.y) * graphic.height),
                        max: viewport.yMax - (movingGraphic.pivot.y * graphic.height)
                    )
            );

            newPosition.x = lockedAxis != LockedAxis.Horizontal ? alteredPos.x : newPosition.x;
            newPosition.y = lockedAxis != LockedAxis.Vertical ? alteredPos.y : newPosition.y;
            //}
            drawRect(graphic, Color.red, 1);
            drawRect(viewport, Color.cyan, 1);
            void drawRect(Rect rect, Color color, float d)
            {
                Debug.DrawLine(rect.min, new Vector2(rect.xMax, rect.yMin), color, d);
                Debug.DrawLine(rect.min, new Vector2(rect.xMin, rect.yMax), color, d);

                Debug.DrawLine(rect.min, new Vector2(rect.xMax, rect.yMin), color, d);
                Debug.DrawLine(rect.max, new Vector2(rect.xMin, rect.yMax), color, d);
            }

            return newPosition;
        }

        private void UpdateScrollRect(Vector3 unclampedPosition, Rect graphic, Rect viewport)
        {
            if (manipulateParentScrollrect && scrollRect)
            {
                graphic.position = unclampedPosition;
                Vector2 v = new Vector2
                    (
                        x: graphic.xMax > viewport.xMax ? -scrollSpeed : (graphic.xMin < viewport.xMin ? scrollSpeed : scrollRect.velocity.x),
                        y: graphic.yMax > viewport.yMax ? -scrollSpeed : (graphic.yMin < viewport.yMin ? scrollSpeed : scrollRect.velocity.y)
                    );
                scrollRect.velocity = v;
            }
        }

        private void Start()
        {
            if (canvas)
            {
                dragContainer = canvas.transform as RectTransform;
            }

            originalParent = movingGraphic.parent;
            originalAnchoredPosition = movingGraphic.anchoredPosition;
        }

        private void Update()
        {
            if (dragging)
            {
                //movingGraphic.position = ClampPosition(transform.position);
                (Rect graphic, Rect viewport) rects = CreateRects();
                UpdateScrollRect(lastUnclampedPosition, rects.graphic, rects.viewport);
            }
        }

        private enum LockedAxis
        {
            Horizontal,
            Vertical,
            None
        }
    }
}
