using UnityEngine;

namespace UI.Pictures
{
    public class PictureCell :  MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        
        public void SetSize(float size)
        {
            rectTransform.sizeDelta = new Vector2(size, size);
        }

        public void SetPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}