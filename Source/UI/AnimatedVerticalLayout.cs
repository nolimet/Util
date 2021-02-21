using System.Collections.Generic;
using UnityEngine;

namespace NoUtil.UI
{
    public class AnimatedVerticalLayout : MonoBehaviour
    {
        [Header("Layout settings")]
        [SerializeField] private Vector2 anchorMin = new Vector2(0, 0);

        [SerializeField] private Vector2 anchorMax = new Vector2(1, 0);
        [SerializeField] private SortingDirection sortingDirection;
        [SerializeField] private float spacingYAxis = 6f;
        private int sortingDirectionModifier = 1;

        [Space]
        [Header("Movement Settings")]
        [SerializeField, Tooltip("Used for when the elements move")] private float unitsPerSecond = 100f;

        [SerializeField, Tooltip("Maxium time a move may take, Elements will try and move at the speed if posible for smaller durations")]
        private float maxDuration = 0.3f;

        [SerializeField] private MovementType movementType = MovementType.FixedDuration;

        [Header("Other Settings")]
        [SerializeField] private bool originalHeightIsMinium;

        [SerializeField] private bool useOnlyActiveChildren = true;
        [SerializeField] private bool snapPositionNewElements = true;

        private List<RectTransform> children = new List<RectTransform>();
        private List<RectTransform> oldChildren = new List<RectTransform>();
        private Dictionary<RectTransform, Vector2> targetPositions = new Dictionary<RectTransform, Vector2>();
        private Vector2 selfOriginalSize;
        private RectTransform rectTransform = null;
        public Vector2 TargetSelfSize { get; private set; } = Vector2.zero;
        public bool LayoutChangingSize => !Mathf.Approximately(rectTransform.rect.height, TargetSelfSize.y) || !Mathf.Approximately(rectTransform.rect.width, TargetSelfSize.x);

        [ContextMenu("Update Position")]
        public void UpdateTargetPositions()
        {
            if (!rectTransform)
            {
                Awake();
            }

            Vector2 newTargetSelfSize = rectTransform.rect.size;

            float currentYPos = 0f;
            RectTransform previousChild = null;
            foreach (var child in children)
            {
                targetPositions[child] = new Vector2(0, currentYPos * sortingDirectionModifier);
                newTargetSelfSize.y = currentYPos + child.sizeDelta.y;
                currentYPos += child.sizeDelta.y + spacingYAxis;
                if (snapPositionNewElements && !oldChildren.Contains(child))
                {
                    child.anchoredPosition = previousChild ? previousChild.anchoredPosition : targetPositions[child];
                }

                previousChild = child;
            }
            if (originalHeightIsMinium)
            {
                newTargetSelfSize.y = TargetSelfSize.y > selfOriginalSize.y ? TargetSelfSize.y : selfOriginalSize.y;
            }

            TargetSelfSize = newTargetSelfSize;
        }

        public void UpdatePositionAndCollectChildren()
        {
            CollectChildren();
        }

        public void SetHeightToTargetHeight()
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TargetSelfSize.y);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TargetSelfSize.x);
        }

        private void Awake()
        {
            rectTransform = transform as RectTransform;
            selfOriginalSize = rectTransform.rect.size;

            if (sortingDirection == SortingDirection.Up)
            {
                sortingDirectionModifier = 1;
            }
            else if (sortingDirection == SortingDirection.Down)
            {
                sortingDirectionModifier = -1;
            }
        }

        private void CollectChildren()
        {
            oldChildren.Clear();
            oldChildren.AddRange(children.ToArray());
            children.Clear();
            targetPositions.Clear();

            foreach (var t in transform)
            {
                if (t is RectTransform currentChild && (!useOnlyActiveChildren || currentChild.gameObject.activeSelf))
                {
                    children.Add(currentChild);
                    targetPositions.Add(currentChild, currentChild.anchoredPosition);
                    currentChild.anchorMin = anchorMin;
                    currentChild.anchorMax = anchorMax;
                    currentChild.pivot = (anchorMax + anchorMin) / 2f;
                    currentChild.sizeDelta = new Vector2(0, currentChild.sizeDelta.y);
                }
            }
            if (sortingDirection == SortingDirection.Up)
            {
                children.Reverse();
            }

            UpdateTargetPositions();
        }

        private void Update()
        {
            if (transform.childCount != children.Count)
            {
                UpdatePositionAndCollectChildren();
            }
            Vector2 newSize = selfOriginalSize;
            switch (movementType)
            {
                case MovementType.Maxspeed:
                    {
                        float maxMoveDelta = unitsPerSecond * Time.deltaTime;
                        foreach (var child in children)
                        {
                            child.anchoredPosition = Vector2.MoveTowards(child.anchoredPosition, targetPositions[child], maxMoveDelta);
                        }
                        newSize = Vector2.MoveTowards(rectTransform.rect.size, TargetSelfSize, maxMoveDelta);
                        break;
                    }

                case MovementType.FixedDuration:
                    {
                        float maxMoveDelta = unitsPerSecond * Time.deltaTime;
                        float maxNormalMoveSpeed = unitsPerSecond * maxDuration;
                        float distance;
                        foreach (var child in children)
                        {
                            distance = Vector2.Distance(child.anchoredPosition, targetPositions[child]);
                            child.anchoredPosition = distance < maxNormalMoveSpeed
                                ? Vector2.MoveTowards(child.anchoredPosition, targetPositions[child], maxMoveDelta)
                                : Vector2.MoveTowards(child.anchoredPosition, targetPositions[child], distance / maxDuration);
                        }

                        distance = Vector2.Distance(rectTransform.rect.size, TargetSelfSize);
                        newSize = distance < maxNormalMoveSpeed
                            ? Vector2.MoveTowards(rectTransform.rect.size, TargetSelfSize, maxMoveDelta)
                            : Vector2.MoveTowards(rectTransform.rect.size, TargetSelfSize, distance / maxDuration);
                        break;
                    }
            }

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
        }

        private enum SortingDirection { Up, Down }

        private enum MovementType { Maxspeed, FixedDuration }
    }
}
