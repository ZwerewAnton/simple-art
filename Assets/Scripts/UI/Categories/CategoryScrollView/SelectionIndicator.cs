using PrimeTween;
using UnityEngine;

namespace UI.Categories.CategoryScrollView
{
    public class SelectionIndicator : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform indicatorRectTransform;
        [SerializeField] private float indicatorTweenDuration = 0.4f;
        private float _elementLength;
        private int _elementsCount;

        private Tween _indicatorTween;

        public void Setup(float size, int elementsCount)
        {
            SetIndicatorSize(size);
            _elementsCount = elementsCount;
            _elementLength = rectTransform.rect.width / _elementsCount;
        }

        public void MoveByIndex(int elementsIndex)
        {
            if (elementsIndex < 0 || elementsIndex >= _elementsCount)
                return;

            if (_indicatorTween.isAlive)
                _indicatorTween.Stop();

            var position = elementsIndex * _elementLength;

            _indicatorTween = Tween.UIAnchoredPosition(
                indicatorRectTransform,
                new Vector2(position, indicatorRectTransform.anchoredPosition.y),
                indicatorTweenDuration,
                Ease.OutCubic
            );
        }

        private void SetIndicatorSize(float size)
        {
            indicatorRectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                size
            );
        }
    }
}