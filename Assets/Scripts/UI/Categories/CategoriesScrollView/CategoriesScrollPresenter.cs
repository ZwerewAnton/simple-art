using System.Collections.Generic;
using UI.Common.ScrollView;
using UI.Pictures;
using UnityEngine;

namespace UI.Categories.CategoriesScrollView
{
    public class CategoriesScrollPresenter : ScrollPresenterBase<CategoriesItemModel, CategoriesItemView>
    {
        public override void Initialize(List<CategoriesItemModel> newModels)
        {
            base.Initialize(newModels);
        }

        protected override void CreatePool()
        {
            base.CreatePool();

            foreach (var itemView in ActiveItems)
            {
                itemView.Initialize();
            }
        }

        protected override float CalculateItemSize(RectTransform rect)
        {
            return GetViewportSize();
        }
    }
}