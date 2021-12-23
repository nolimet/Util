//Source https://gitlab.com/jonnohopkins/tmp-hyperlinks/-/blob/master/Assets/OpenHyperlinks.cs

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// somewhat based upon the TextMesh Pro example script: TMP_TextSelector_B
[RequireComponent(typeof(TextMeshProUGUI))]
public class OpenHyperlinks : MonoBehaviour, IPointerClickHandler
{
    public bool doesColorChangeOnHover = true;
    public Color hoverColor = new Color(60f / 255f, 120f / 255f, 1f);

    private TextMeshProUGUI textfield;
    private Canvas canvas;
    private Camera camera;

    public bool isLinkHighlighted => currentLink != -1;

    private int currentLink = -1;
    private List<Color32[]> originalVertexColors = new List<Color32[]>();

    protected virtual void Awake()
    {
        textfield = GetComponent<TextMeshProUGUI>();
        canvas = GetComponentInParent<Canvas>();

        // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
        camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }

    private void LateUpdate()
    {
        // is the cursor in the correct region (above the text area) and furthermore, in the link region?
        var isHoveringOver = TMP_TextUtilities.IsIntersectingRectTransform(textfield.rectTransform, Input.mousePosition, camera);
        int linkIndex = isHoveringOver ? TMP_TextUtilities.FindIntersectingLink(textfield, Input.mousePosition, camera) : -1;

        // Clear previous link selection if one existed.
        if (currentLink != -1 && linkIndex != currentLink)
        {
            // Debug.Log("Clear old selection");
            SetLinkToColor(currentLink, (linkIdx, vertIdx) => originalVertexColors[linkIdx][vertIdx]);
            originalVertexColors.Clear();
            currentLink = -1;
        }

        // Handle new link selection.
        if (linkIndex != -1 && linkIndex != currentLink)
        {
            // Debug.Log("New selection");
            currentLink = linkIndex;
            if (doesColorChangeOnHover)
                originalVertexColors = SetLinkToColor(linkIndex, (_linkIdx, _vertIdx) => hoverColor);
        }

        // Debug.Log(string.Format("isHovering: {0}, link: {1}", isHoveringOver, linkIndex));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("Click at POS: " + eventData.position + "  World POS: " + eventData.worldPosition);

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textfield, Input.mousePosition, camera);
        if (linkIndex != -1)
        { // was a link clicked?
            TMP_LinkInfo linkInfo = textfield.textInfo.linkInfo[linkIndex];

            // Debug.Log(string.Format("id: {0}, text: {1}", linkInfo.GetLinkID(), linkInfo.GetLinkText()));
            // open the link id as a url, which is the metadata we added in the text field
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }

    private List<Color32[]> SetLinkToColor(int linkIndex, Func<int, int, Color32> colorForLinkAndVert)
    {
        TMP_LinkInfo linkInfo = textfield.textInfo.linkInfo[linkIndex];

        var oldVertColors = new List<Color32[]>(); // store the old character colors

        for (int i = 0; i < linkInfo.linkTextLength; i++)
        { // for each character in the link string
            int characterIndex = linkInfo.linkTextfirstCharacterIndex + i; // the character index into the entire text
            var charInfo = textfield.textInfo.characterInfo[characterIndex];
            int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.
            int vertexIndex = charInfo.vertexIndex; // Get the index of the first vertex of this character.

            Color32[] vertexColors = textfield.textInfo.meshInfo[meshIndex].colors32; // the colors for this character
            oldVertColors.Add(vertexColors.ToArray());

            if (charInfo.isVisible)
            {
                vertexColors[vertexIndex + 0] = colorForLinkAndVert(i, vertexIndex + 0);
                vertexColors[vertexIndex + 1] = colorForLinkAndVert(i, vertexIndex + 1);
                vertexColors[vertexIndex + 2] = colorForLinkAndVert(i, vertexIndex + 2);
                vertexColors[vertexIndex + 3] = colorForLinkAndVert(i, vertexIndex + 3);
            }
        }

        // Update Geometry
        textfield.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        return oldVertColors;
    }
}