using System.Collections.Generic;
using UI.Common.ScrollView;
using UI.Pictures;
using UnityEngine;
using Zenject;

namespace UI.Categories.CategoriesScrollView
{
    public class CategoriesScrollPresenter : ScrollPresenterBase<CategoriesItemModel, CategoriesItemView>
    {
        private DiContainer _diContainer;
        
        [Inject]
        public void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        public override void Initialize(List<CategoriesItemModel> newModels)
        {
            base.Initialize(newModels);
        }

        protected override void CreatePool()
        {
            ClearPool();

            for (var i = 0; i < ViewItemCount; i++)
            {
                var go = _diContainer.InstantiatePrefab(itemPrefab, content);
                var item = go.GetComponent<CategoriesItemView>();
                SetupItemRectTransform(item.RectTransform);
                item.Clicked += OnItemClicked;
                item.Initialize();
                ActiveItems.Add(item);
            }
        }

        protected override float CalculateItemSize(RectTransform rect)
        {
            return GetViewportSize();
        }
    }
}