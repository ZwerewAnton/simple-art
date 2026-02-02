using PrimeTween;
using UnityEngine;

namespace UI.Categories.CategoryScrollView
{
    public class SelectionIndicator : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float indicatorTweenDuration = 0.4f;
        
        private Tween _indicatorTween;
        
        public void MoveSelectionIndicator(float position)
        {
            if (_indicatorTween.isAlive)
                _indicatorTween.Stop();

            _indicatorTween = Tween.UIAnchoredPosition(
                rectTransform,
                new Vector2(position, rectTransform.anchoredPosition.y),
                indicatorTweenDuration,
                Ease.OutCubic
            );
        }

        public void SetIndicatorSize(float size)
        {
            rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                size
            );
        }
    }
}
