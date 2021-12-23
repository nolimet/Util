using UnityEngine;

namespace NoUtil.Extentsions
{
    public static class SpriteRendererExtentsions
    {
        public static void ScaleSpriteToFitScreen(SpriteRenderer spriteRenderer, bool preserveAspect)
        {
            Vector3 newScale = Vector3.one;

            float width = spriteRenderer.sprite.bounds.size.x;
            float height = spriteRenderer.sprite.bounds.size.y;

            float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            newScale.x = worldScreenWidth / width;
            newScale.y = worldScreenHeight / height;

            if (preserveAspect)
            {
                if (newScale.x > newScale.y)
                {
                    newScale.y = newScale.x;
                }
                else
                {
                    newScale.x = newScale.y;
                }
            }

            spriteRenderer.transform.localScale = newScale;
        }
    }
}