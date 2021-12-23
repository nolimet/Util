using UnityEngine;
using System.Linq;

namespace NoUtil.UI
{
    public class GridlayoutWidthSetter : MonoBehaviour
    {
        private RectTransform rectTransform;
        private UnityEngine.UI.GridLayoutGroup gridLayoutGroup;
        private CustomGrid customGrid;
        public int ChildrenNeededToScroll = 8;
        public bool onlySetContainerHeight = true;
        public bool onlyUseActiveChildren = true;
        private int childrenLast = 0;
        private int ChildrenCount = 0;
        private Vector2 spacing, cellSize;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;

            if (GetComponent<UnityEngine.UI.GridLayoutGroup>())
            {
                gridLayoutGroup = GetComponent<UnityEngine.UI.GridLayoutGroup>();
                gridLayoutGroup.cellSize = new Vector2(rectTransform.rect.width, gridLayoutGroup.cellSize.y);
                spacing = gridLayoutGroup.spacing;
                cellSize = gridLayoutGroup.cellSize;
            }

            if (GetComponent<CustomGrid>())
            {
                customGrid = GetComponent<CustomGrid>();
                if (!onlySetContainerHeight)
                    customGrid.ObjSize = new Vector2(rectTransform.rect.width, customGrid.ObjSize.y);
                cellSize = customGrid.ObjSize;
                spacing = customGrid.maxSpacing;
            }
        }

        public void ForceUpdate()
        {
            ChildrenCount = GetActiveChildCount();
            if (ChildrenCount > ChildrenNeededToScroll)
            {
                if (!customGrid)
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (cellSize.y + spacing.y) * ChildrenCount);
                else
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (customGrid.ObjSize.y + (customGrid.padding.y + customGrid.CurrentSpacing.y)) * Mathf.CeilToInt(ChildrenCount / (float)customGrid.maxColumns));
            }
            else
            {
                if (!customGrid)
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (cellSize.y + spacing.y) * ChildrenNeededToScroll);
                else
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (customGrid.ObjSize.y + (customGrid.padding.y + customGrid.CurrentSpacing.y)) * (ChildrenNeededToScroll / customGrid.maxColumns));
            }
        }

        private void Update()
        {
            ChildrenCount = GetActiveChildCount();
            if (childrenLast != ChildrenCount)
            {
                ForceUpdate();
            }
        }

        private int GetActiveChildCount() => (from Transform transform in transform
                                              where transform.gameObject.activeSelf
                                              select transform).Count();
    }
}