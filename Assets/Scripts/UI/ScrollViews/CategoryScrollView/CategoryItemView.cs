using PrimeTween;
using TMPro;
using UI.Common.ScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ScrollViews.CategoryScrollView
{
    public class CategoryItemView : ScrollItemView<CategoryItemModel>
    {
        [SerializeField] private TMP_Text categoryName;
        [SerializeField] private Image separator;
        
        private Tween _colorTween;
        
        public override void SetData(int itemIndex, CategoryItemModel model)
        {
            base.SetData(itemIndex, model);
            
            categoryName.SetText(model.categoryName);
            separator.gameObject.SetActive(!model.isLast);
            AnimateTextColor(model.isSelected);
        }
        
        private void AnimateTextColor(bool isSelected)
        {
            var targetColor = isSelected ? Color.red : Color.black;

            if (categoryName.color == targetColor)
                return;

            if (_colorTween.isAlive)
                _colorTween.Stop();

            _colorTween = Tween.Color(
                target: categoryName,
                endValue: targetColor,
                duration: 0.4f,
                ease: Ease.OutQuad
            );
        }

        private void OnDisable()
        {
            if (_colorTween.isAlive)
                _colorTween.Stop();
        }
    }
}