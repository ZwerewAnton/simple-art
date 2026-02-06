using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Common.ScrollView
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class ScrollItemView<TModel> : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform rectTransform;
        public int ItemIndex { get; set; } = -1;
        public bool IsRefreshed { get; set; }

        public RectTransform RectTransform => rectTransform;
        
        public bool Active => gameObject.activeSelf;

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(ItemIndex);
        }

        public event Action<int> Clicked;

        public virtual void SetData(int itemIndex, TModel model)
        {
            ItemIndex = itemIndex;
        }

        public void SetAnchoredPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }

        public void SetActive(bool active)
        {
            if (gameObject.activeSelf == active) 
                return;
            
            gameObject.SetActive(active);
            IsRefreshed = false;
        }
    }
}