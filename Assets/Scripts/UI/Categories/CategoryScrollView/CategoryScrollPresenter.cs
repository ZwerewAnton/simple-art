using System;
using System.Linq;
using UI.Common.ScrollView;
using UI.ScrollViews.CategoryScrollView;
using UnityEngine;

namespace UI.Categories.CategoryScrollView
{
    public class CategoryScrollPresenter : ScrollPresenterBase<CategoryItemModel, CategoryItemView>
    {
        public event Action<int> ItemClicked;

        public void SelectModel(int index)
        {
            if (index < 0 && index >= Models.Count)
                return;

            for (var i = 0; i < Models.Count; i++) Models[i].isSelected = index == i;

            MarkViewsToRefresh();
            MarkToUpdate();
        }

        protected override float CalculateItemSize(RectTransform rect)
        {
            var rectHeight = rect.rect.height;
            var cellSize = rectHeight + itemSpacing;
            var viewportSize = GetViewportSize();
            if (Mathf.CeilToInt(viewportSize / cellSize) > Models.Count) return viewportSize / Models.Count;

            return base.CalculateItemSize(rect);
        }

        protected override void CreatePool()
        {
            base.CreatePool();

            foreach (var categoryItemView in ActiveItems)
                categoryItemView.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ItemSize);
        }

        protected override void OnItemClicked(int itemIndex)
        {
            base.OnItemClicked(itemIndex);

            if (itemIndex >= Models.Count)
                return;

            ItemClicked?.Invoke(itemIndex);

            for (var index = 0; index < Models.Count; index++) Models[index].isSelected = index == itemIndex;

            var item = ActiveItems.FirstOrDefault(i => i.ItemIndex == itemIndex);
            if (item == null)
                return;

            TargetItemData = new TargetItemData(item.RectTransform.anchoredPosition, item.ItemIndex);
            MarkToUpdate();

            ShouldSnap = true;
        }
    }
}