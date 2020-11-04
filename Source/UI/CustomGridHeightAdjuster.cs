using UnityEngine;

//using UnityEngine.UI;

namespace NoUtil.UI
{
    [RequireComponent(typeof(CustomGrid), typeof(RectTransform))]
    public class CustomGridHeightAdjuster : MonoBehaviour//, ILayoutElement
    {
        private CustomGrid grid;
        private RectTransform rect;
        private float height = 0f;
        private float extraHeight = 0f;

        public void AddExtraHeight(float f)
        {
            extraHeight = f > 0 ? f : 0;
            CalcHeight();
        }

        public void RemoveExtraHeight()
        {
            extraHeight = 0f;
            CalcHeight();
        }

        public void ForceUpdate()
        {
            grid.ForceUpdate();
            CalcHeight();
        }

        private void Awake()
        {
            grid = GetComponent<CustomGrid>();
            rect = (RectTransform)transform;
        }

        private void Update()
        {
            CalcHeight();
        }

        private void CalcHeight()
        {
            if (!rect)
            {
                rect = (RectTransform)transform;
            }

            if (!grid)
            {
                grid = GetComponent<CustomGrid>();
            }
            if (!grid)
            {
                return;
            }

            height = ((grid.ObjSize.y + grid.padding.y) * grid.CurrentRows) + (grid.CurrentSpacing.y * ((grid.CurrentRows - 1f)));
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height + extraHeight);
        }
    }
}
