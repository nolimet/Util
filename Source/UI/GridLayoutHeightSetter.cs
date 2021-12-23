using UnityEngine;

namespace NoUtil.UI
{
    public class GridLayoutHeightSetter : MonoBehaviour
    {
        public bool onlyUseActive;
        private float orignalHeight;

        // Use this for initialization
        private void Start()
        {
            orignalHeight = ((RectTransform)transform).sizeDelta.y;
        }

        private void Update()
        {
            if (GetComponent<UnityEngine.UI.GridLayoutGroup>() && GetComponent<UnityEngine.UI.GridLayoutGroup>().preferredHeight > orignalHeight)
                ((RectTransform)transform).sizeDelta = new Vector2(((RectTransform)transform).sizeDelta.x, transform.childCount * (GetComponent<UnityEngine.UI.GridLayoutGroup>().cellSize.y + GetComponent<UnityEngine.UI.GridLayoutGroup>().spacing.y));
            if (GetComponent<CustomGrid>())
            {
                CustomGrid c = GetComponent<CustomGrid>();
                ((RectTransform)transform).sizeDelta = new Vector2(((RectTransform)transform).sizeDelta.x, c.transform.childCount * (c.ObjSize.y));
                //((RectTransform)transform).
            }
        }
    }
}