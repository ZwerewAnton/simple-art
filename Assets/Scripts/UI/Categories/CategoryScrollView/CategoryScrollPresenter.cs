using System.Collections.Generic;
using System.Linq;
using UI.Common.ScrollView;
using UI.ScrollViews;
using UI.ScrollViews.CategoryScrollView;
using UnityEngine;

namespace UI.Categories.CategoryScrollView
{
    public class CategoryScrollPresenter : ScrollPresenterBase<CategoryItemModel, CategoryItemView>
    {
        [Header("Snap")] [SerializeField] [Range(0f, 20f)]
        protected float snapSpeed = 10f;
        [SerializeField] protected float snapThreshold = 0.5f;
        [SerializeField] protected SelectionIndicator selectionIndicator;
        
        private Color _defaultColor = Color.black;
        private Color _highlightColor = Color.red;
        
        private TargetItemData _targetItemData;
        private bool _shouldSnap;
        private bool enableSnap;

        public override void Initialize(List<CategoryItemModel> newModels)
        {
            base.Initialize(newModels);

            selectionIndicator.SetIndicatorSize(ItemSize);
        }
        protected override float CalculateItemSize(RectTransform rect)
        {
            var rectHeight = rect.rect.height;
            var cellSize = rectHeight + itemSpacing;
            var viewportSize = GetViewportSize();
            if (Mathf.CeilToInt(viewportSize / cellSize) > Models.Count)
            {
                enableSnap = false;
                return viewportSize / Models.Count;
            }
            
            return base.CalculateItemSize(rect);
        }

        protected override void CreatePool()
        {
            base.CreatePool();

            foreach (var categoryItemView in ActiveItems)
            {
                categoryItemView.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ItemSize);
            }
        }
        
        protected void LateUpdate()
        {
            if (_shouldSnap && enableSnap)
                SmoothSnap();
        }
        
        protected override void OnItemClicked(int itemIndex)
        {
            base.OnItemClicked(itemIndex);
            
            if (itemIndex >= Models.Count)
                return;

            for (var index = 0; index < Models.Count; index++)
            {
                Models[index].isSelected = index == itemIndex;
            }

            var item = ActiveItems.FirstOrDefault(i => i.ItemIndex == itemIndex);
            if (item == null)
                return;

            _targetItemData = new TargetItemData(item.RectTransform.anchoredPosition, item.ItemIndex);
            MarkToUpdate();
            
            selectionIndicator.MoveSelectionIndicator(itemIndex * (ItemSize + itemSpacing));
            
            _shouldSnap = true;
        }
        
        private void SmoothSnap()
        {
            var currentX = content.anchoredPosition.x;
            var targetX = -_targetItemData.AnchoredPosition.x + BorderSpacing;

            var newX = Mathf.Lerp(currentX, targetX, snapSpeed * Time.deltaTime);
            content.anchoredPosition = new Vector2(newX, content.anchoredPosition.y);

            if (Mathf.Abs(newX - targetX) < snapThreshold)
            {
                content.anchoredPosition = new Vector2(targetX, content.anchoredPosition.y);
                _shouldSnap = false;
            }
        }
    }
}