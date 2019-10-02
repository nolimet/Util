using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Util.UI
{
    public class Scrolldown : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField, Tooltip("Scroll speed is in uiUnits per Second")]
        private float scrollSpeed = 500;

        [SerializeField]
        private AnimationCurve scrollAccelerationCurve = new AnimationCurve(new Keyframe(0, 0.2f), new Keyframe(0.1f, .5f), new Keyframe(0.3f, 1f), new Keyframe(.95f, 1f), new Keyframe(1, 0.2f));

        [SerializeField]
        private float bottomValue = 0;

        [SerializeField]
        Button scrollDownButton;

        bool scrollDown;

        float startPosScroll;

        private float ScrollRectPos { get => Mathf.Round(scrollRect.verticalNormalizedPosition * 10000) / 10000; set => scrollRect.verticalNormalizedPosition = value; }
        private void Update()
        {
            if (scrollDown)
            {
                scrollRect.velocity = new Vector2(0, scrollAccelerationCurve.Evaluate((startPosScroll - ScrollRectPos) / startPosScroll) * scrollSpeed);

                if (ScrollRectPos <= 0.0001f)
                    scrollDown = false;
            }
            scrollDownButton.interactable = !scrollDown && scrollRect.content.rect.height > scrollRect.viewport.rect.height && ScrollRectPos > 0.0001f;
        }

        public void ScrollDown()
        {
            startPosScroll = ScrollRectPos;
            scrollDown = true;
        }
    }
}
