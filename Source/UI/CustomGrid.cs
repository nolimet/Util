using System.Collections.Generic;
using UnityEngine;

namespace NoUtil.UI
{
    /// <summary>
    /// A grid sorting method that has the abilty create simple soothend animations. It also only
    /// does things when it detects changes making it quite light;
    /// </summary>
    public class CustomGrid : MonoBehaviour
    {
        /// <summary>
        /// Size that each object will have
        /// </summary>
        public Vector2 ObjSize;

        /// <summary>
        /// forced space between each object
        /// </summary>
        public Vector2 padding;

        /// <summary>
        /// the maxium space between two objects
        /// </summary>
        public Vector2 maxSpacing;

        /// <summary>
        /// The anchor point or from wich the object wil be centered
        /// </summary>
        public Vector2 AnchorPoint = Vector2.zero;

        /// <summary>
        /// The current space between the objects
        /// </summary>
        [ReadOnly]
        public Vector2 CurrentSpacing = Vector2.zero;

        /// <summary>
        /// The maxium number of rows
        /// </summary>
        [Range(1, 500)] //can ve changed to anynumber above 0
        public int maxColumns = 1;

        /// <summary>
        /// Start placing objects on the leftside
        /// </summary>
        public bool StartLeft = true;

        /// <summary>
        /// The minum objecs on a single row needed before it creates a second row
        /// </summary>
        [Range(1, 500)]
        public int minNeededToStartSecondRow = 1;

        /// <summary>
        /// The y offset used for when the objects are not well centered them selfs
        /// </summary>
        [Range(0, 1f)]
        public float yoffset;

        /// <summary>
        /// Does the grid update continuesly. usefull when debugging the grid or seeing if everything
        /// works correctly. Recommened to have it turned of when you are building the game because
        /// it saves preformance
        /// </summary>
        public bool continuesUpdates = false;

        public bool autoFitMaxOnRow = true;

        /// <summary>
        /// Enable the animations so that the elements move towards the new points instead of teleporting
        /// </summary>
        public bool useAnimations = false;

        /// <summary>
        /// used for the update tigger. When the number of childeren changes
        /// </summary>
        private int childCountLast = 0;

        /// <summary>
        /// Are they at the objects at there new positions
        /// </summary>
        private bool atNewPos = false;

        /// <summary>
        /// Used for the animations so they run smoothly
        /// </summary>
        private float StartTime;

        /// <summary>
        /// List that contains positional data for the objects
        /// </summary>
        private List<Vector2> newPos = new List<Vector2>(), startPos = new List<Vector2>();

        /// <summary>
        /// List of active game objects
        /// </summary>
        [ReadOnly, SerializeField]
        private List<RectTransform> ActiveChildren;

        /// <summary>
        /// Toggle to only use enabled game ojbects
        /// </summary>
        [SerializeField]
        private bool onlyUseActiveObjects;

        private RectTransform t;

        public int CurrentRows
        {
            get; private set;
        }

        public Vector2 RecommendedSize
        {
            get; private set;
        }

        public void ForceUpdate()
        {
            OnEnable();
        }

        private void Start()
        {
            t = (RectTransform)transform;
            atNewPos = false;
            ActiveChildren = new List<RectTransform>();
        }

        private void OnEnable()
        {
            if (autoFitMaxOnRow)
            {
                CalcMaxRow();
            }
            SetChildPosition();
            PostProcessPositions();
            SetChildSize();
            SnapToPos();

            atNewPos = false;
        }

        private void Awake()
        {
            atNewPos = false;
        }

        private void Update()
        {
            if (!enabled)
            {
                return;
            }

            if (childCountLast != GetChildCount())
            {
                if (autoFitMaxOnRow)
                {
                    CalcMaxRow();
                }
                SetChildSize();
                SetChildPosition();
                PostProcessPositions();
                childCountLast = GetChildCount();
            }
            else if (atNewPos && continuesUpdates)
            {
                SetChildSize();
                SetChildPosition();
                PostProcessPositions();
                SnapToPos();
            }
            if (!atNewPos)
            {
                if ((Time.time - StartTime) * 4f <= 1f && useAnimations)
                {
                    MoveObjectsToPos();
                }
                else
                {
                    atNewPos = true;
                    SnapToPos();
                }
            }
        }

        private void SetChildSize()
        {
            RectTransform child;
            for (int i = 0; i < transform.childCount; i++)
            {
                child = (RectTransform)t.GetChild(i);
                child.sizeDelta = ObjSize;
                // child.name = "Obj " + i.ToString();
                //child.anchorMax = Vector2.one / 2;
                //child.anchorMin = Vector2.one / 2;
                child.anchorMax = AnchorPoint;
                child.anchorMin = AnchorPoint;
            }
        }

        private void SetChildPosition()
        {
            if (transform.childCount == 0)
            {
                return;
            }

            StartTime = Time.time;
            atNewPos = false;
            startPos.Clear();
            newPos.Clear();
            ActiveChildren.Clear();

            if (!t)
            {
                t = (RectTransform)transform;
            }

            Rect WH = t.rect;
            int noOfChilds = GetChildCount();
            float spacingY, spacingX;
            RectTransform Child;
            float indexLocal = 0;
            float z = 0;
            int column = 0;
            Vector2 pos = Vector2.zero;

            ///calculate HorizontalSpace
            if (noOfChilds == 1)
            {
                spacingY = 0;
            }
            else if (noOfChilds >= minNeededToStartSecondRow) //diffrence less than 3 and 3 is special alignment
            {
                spacingY = WH.height - ((Mathf.CeilToInt(noOfChilds / (float)maxColumns)) * ObjSize.y);
            }
            else
            {
                spacingY = WH.height - (ObjSize.y * noOfChilds);
            }

            if (noOfChilds < minNeededToStartSecondRow)
            {
                spacingX = 0;
            }
            else
            {
                spacingX = WH.width - (maxColumns * ObjSize.x);
            }

            spacingX /= maxColumns;
            spacingY /= Mathf.CeilToInt(noOfChilds / (float)maxColumns);

            if (spacingX < 0)
            {
                spacingX = 0;
            }

            if (spacingY < 0)
            {
                spacingY = 0;
            }

            if (spacingY > maxSpacing.y)
            {
                spacingY = maxSpacing.y;
            }

            if (spacingX > maxSpacing.x)
            {
                spacingX = maxSpacing.x;
            }
            z = (spacingX + ObjSize.x);
            CurrentSpacing = new Vector2(spacingX, spacingY);
            spacingX += padding.x;
            spacingY += padding.y;

            for (int i = 0; i < noOfChilds; i++)
            {
                Child = ActiveChildren[i];

                if (!onlyUseActiveObjects || onlyUseActiveObjects && Child.gameObject.activeSelf)
                {
                    if (i % maxColumns == 0)
                    {
                        column = StartLeft ? 1 : maxColumns;
                    }

                    if (noOfChilds <= minNeededToStartSecondRow)
                    {
                        indexLocal = (noOfChilds / 2 - i);
                        if (noOfChilds == 2)
                        {
                            indexLocal -= 0.5f;
                        }

                        if (noOfChilds == 0)
                        {
                            indexLocal = 0;
                        }

                        pos = new Vector2(0f, indexLocal * (spacingY + ObjSize.y));
                    }
                    else
                    {
                        if (AnchorPoint.y < 1)
                        {
                            indexLocal = ((noOfChilds / maxColumns) / 2f) - (i / maxColumns);
                        }
                        else
                        {
                            indexLocal = (-i / maxColumns);
                            // Debug.Log(indexLocal);
                        }

                        if (noOfChilds % maxColumns == 0)
                        {
                            indexLocal -= yoffset;
                        }

                        pos = new Vector2(0f, indexLocal * (spacingY + ObjSize.y));

                        if (maxColumns > 1)
                        {
                            pos.x = (z * column) - spacingX - (ObjSize.x * (1 - t.pivot.x));
                        }
                        else
                        {
                            pos.x = 0;
                        }
                    }
                    pos.x = Mathf.Ceil(pos.x);
                    pos.y = Mathf.Ceil(pos.y);
                    startPos.Add(Child.anchoredPosition);
                    newPos.Add(pos);

                    column += StartLeft ? 1 : -1;
                }
            }
            CurrentRows = Mathf.CeilToInt(noOfChilds / (float)maxColumns);
        }

        private void PostProcessPositions()
        {
            if (newPos.Count <= 1 || transform.childCount == 0 || newPos.Count != transform.childCount)
            {
                return;
            }

            float f = padding.x + CurrentSpacing.x;
            float d = ObjSize.x + f;
            f *= AnchorPoint.x;
            d *= AnchorPoint.x;

            Dictionary<float, int> rowLength = new Dictionary<float, int>();
            foreach (Vector2 v in newPos)
            {
                if (!rowLength.ContainsKey(v.y))
                {
                    rowLength.Add(v.y, 0);
                }
                rowLength[v.y]++;
            }
            float OffsetX = 0;
            for (int i = 0; i < newPos.Count; i++)
            {
                OffsetX = (1 - ((RectTransform)transform.GetChild(i)).pivot.x) * ObjSize.x;
                newPos[i] = new Vector2(newPos[i].x - (d * rowLength[newPos[i].y] - f) - (rowLength[newPos[i].y] != maxColumns ? padding.x : 0) - OffsetX, newPos[i].y);
            }
        }

        private void MoveObjectsToPos()
        {
            RectTransform child;
            for (int i = 0; i < ActiveChildren.Count; i++)
            {
                child = ActiveChildren[i];
                child.anchoredPosition = Vector2.Lerp(startPos[i], newPos[i], (Time.time - StartTime) * 4f);
                RoundPos(child);
            }
        }

        private void RoundPos(RectTransform t)
        {
            Vector2 p = t.anchoredPosition;
            t.anchoredPosition = new Vector2(Mathf.Round(p.x), Mathf.Round(p.y));
        }

        private void SnapToPos()
        {
            if (transform.childCount != 0)
            {
                RectTransform child;
                for (int i = 0; i < ActiveChildren.Count; i++)
                {
                    child = ActiveChildren[i];
                    child.anchoredPosition = newPos[i];
                }
            }
        }

        private int GetChildCount()
        {
            ActiveChildren.Clear();
            for (int i = 0; i < t.childCount; i++)
            {
                if (!onlyUseActiveObjects || onlyUseActiveObjects && t.GetChild(i).gameObject.activeSelf)
                {
                    ActiveChildren.Add((RectTransform)t.GetChild(i));
                }
            }

            if (onlyUseActiveObjects)
            {
                int r = 0;
                for (int i = 0; i < t.childCount; i++)
                {
                    if (t.GetChild(i).gameObject.activeSelf)
                    {
                        r++;
                    }
                }

                return r;
            }
            return t.childCount;
        }

        private void CalcMaxRow()
        {
            float width = ((RectTransform)transform).rect.size.x;
            float totalElementWidth = ObjSize.x + padding.x;

            maxColumns = Mathf.FloorToInt(width / totalElementWidth);

            if (maxColumns == 0)
            {
                maxColumns = 1;
            }
        }
    }
}
