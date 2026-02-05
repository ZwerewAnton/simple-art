using System;
using UI.Common.ScrollView;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.Categories.CategoriesScrollView
{
    public class CategoriesScrollPresenter : ScrollPresenterBase<CategoriesItemModel, CategoriesItemView>
    {
        public event Action<int> CenteredViewChanged;
        
        private DiContainer _diContainer;
        private int _lastCenteredItemIndex;
        
        [Inject]
        public void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
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

        protected override void SetupItemRectTransform(RectTransform rect)
        {
            base.SetupItemRectTransform(rect);
            rect.sizeDelta = new Vector2(ItemSize, scrollRect.viewport.rect.height);
        }

        protected override float CalculateItemSize(RectTransform rect)
        {
            return GetViewportSize();
        }
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            SaveViewsPosition();
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            SaveViewsPosition();
        }

        
        protected override void OnSnapEnded()
        {
            SaveViewsPosition();
        }

        protected override void OnScrollChanged(Vector2 _)
        {
            base.OnScrollChanged(_);

            CheckCenteredViewChanging();
        }

        public void MoveToItem(int index)
        {
            if (index < 0 && index >= Models.Count)
                return;
            
            EnableSnap();
            TargetItemData = new TargetItemData(GetAnchoredPosition(index), index);
            SaveViewsPosition();
        }

        private void CheckCenteredViewChanging()
        {
            var center = -content.anchoredPosition.x;
            var closestDist = float.MaxValue;
            var closestModelIndex = 0;
            
            foreach (var item in ActiveItems)
            {
                var itemCenter = item.RectTransform.anchoredPosition.x - BorderSpacing;
                var distance = center - itemCenter;

                var absDist = Mathf.Abs(distance);
                if (absDist < closestDist)
                {
                    closestDist = absDist;
                    closestModelIndex = item.ItemIndex;
                }
            }

            if (_lastCenteredItemIndex != closestModelIndex)
            {
                CenteredViewChanged?.Invoke(closestModelIndex);
                _lastCenteredItemIndex = closestModelIndex;
            }
        }

        private void SaveViewsPosition()
        {
            foreach (var item in ActiveItems)
            {
                if (!item.Active) 
                    continue;
                
                var index = item.ItemIndex;
                
                if (index < 0)
                    continue;
                
                Models[index].contentPosition = item.GetContentPosition();
            }
        }
    }
}