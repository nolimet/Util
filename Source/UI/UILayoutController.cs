using NoUtil.Extentsions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NoUtil.UI
{
    public class UILayoutController : MonoBehaviour
    {
        [SerializeField]
        private float alphaActive = 1, alphaInactive = 0;

        [SerializeField]
        private string defaultLayout = string.Empty;

        [SerializeField]
        private LayoutGroup[] layoutGroups;

        private LayoutGroup activeLayoutGroup;

        private CancellationTokenSource tokenSource;

        public void ChangeLayout(GameObject newLayoutRoot)
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                tokenSource = null;
            }

            tokenSource = new CancellationTokenSource();
            if (newLayoutRoot && layoutGroups.Any(x => x.LayoutRoot == newLayoutRoot))
            {
                ChangeLayoutAsync(newLayoutRoot, tokenSource.Token).ExcecuteTask();
            }
        }

        public async Task ChangeLayoutAsync(GameObject newLayoutRoot, CancellationToken token)
        {
            if (newLayoutRoot && layoutGroups.Any(x => x.LayoutRoot == newLayoutRoot) && activeLayoutGroup.LayoutRoot != newLayoutRoot)
            {
                LayoutGroup newLayoutGroup = layoutGroups.First(x => x.LayoutRoot == newLayoutRoot); // find the new layout

                if (newLayoutGroup.ChangeDuration <= 0)
                {
                    newLayoutGroup.SetActiveInstant(true, alphaActive);
                    activeLayoutGroup.SetActiveInstant(false, alphaInactive);
                }
                else
                {
                    //Create tasks that will run in the until
                    Task changeTaskNewLayout = newLayoutGroup.ChangeLayoutTask(newLayoutGroup.ChangeDuration, alphaActive, true, token);
                    Task changeTaskActiveLayout = activeLayoutGroup.ChangeLayoutTask(newLayoutGroup.ChangeDuration, alphaInactive, false, token);

                    //Wait until the tasks are done
                    await new WaitUntil(WaitUntilTask);
                    bool WaitUntilTask()
                    {
                        return TaskCheck(changeTaskNewLayout) || TaskCheck(changeTaskActiveLayout);
                    }

                    await AsyncAwaiters.NextFrame;
                    if (TaskCheck(changeTaskNewLayout))
                        changeTaskNewLayout.Dispose();
                    if (TaskCheck(changeTaskActiveLayout))
                        changeTaskActiveLayout.Dispose();
                }
                activeLayoutGroup = newLayoutGroup;
            }

            bool TaskCheck(Task task) => task.IsCompleted || task.IsFaulted || task.IsCanceled;
        }

        private void Awake()
        {
            layoutGroups = layoutGroups.Where(x => x.LayoutRoot).ToArray(); // getting rid of any empty groups
            foreach (LayoutGroup layoutGroup in layoutGroups)
            {
                layoutGroup.Init();
                if (layoutGroup.Name == defaultLayout)
                {
                    activeLayoutGroup = layoutGroup;
                    layoutGroup.SetActiveInstant(true, alphaActive);
                }
                else
                {
                    layoutGroup.SetActiveInstant(false, alphaInactive);
                }
            }
        }

        [System.Serializable]
        public class LayoutGroup
        {
            [SerializeField]
            private float changeDuration = 0.2f;

            [SerializeField]
            private GameObject layoutRoot;

            private CanvasGroup canvasGroup;

            public GameObject LayoutRoot => layoutRoot;
            public string Name => layoutRoot ? layoutRoot.name : string.Empty;
            public float ChangeDuration => changeDuration;

            public async Task ChangeLayoutTask(float duration, float targetAlpha, bool isEnabled, CancellationToken token)
            {
                targetAlpha = Mathf.Clamp01(targetAlpha);

                float maxDelta = (isEnabled ? targetAlpha - canvasGroup.alpha : canvasGroup.alpha - targetAlpha) / duration;
                try
                {
                    if (!layoutRoot.activeSelf)
                    {
                        layoutRoot.SetActive(true);
                    }

                    while (Mathf.Abs(canvasGroup.alpha - targetAlpha) >= float.Epsilon)
                    {
                        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, maxDelta * Time.deltaTime);
                        await AsyncAwaiters.NextFrame;
                        token.ThrowIfCancellationRequested();
                    }

                    Debug.Log($"Change completed - {Name}");
                }
                catch (OperationCanceledException e)
                {
                    Debug.LogWarning(e);
                }
                catch (Exception e)
                {
                    canvasGroup.alpha = targetAlpha;
                    Debug.LogException(e);
                    throw;
                }
                finally
                {
                    if (!isEnabled)
                    {
                        layoutRoot.SetActive(false);
                    }
                    canvasGroup.alpha = targetAlpha;
                }
            }

            public void SetActiveInstant(bool active, float alpha)
            {
                canvasGroup.alpha = alpha;
                layoutRoot.SetActive(active);
            }

            public void Init()
            {
                canvasGroup = layoutRoot.GetOrAddComponent<CanvasGroup>();
            }

            public static implicit operator GameObject(LayoutGroup group) => group.LayoutRoot;
        }
    }
}