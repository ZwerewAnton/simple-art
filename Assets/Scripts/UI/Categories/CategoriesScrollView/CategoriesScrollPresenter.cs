using System.Collections.Generic;
using UI.Common.ScrollView;
using UI.ScrollViews;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.Categories.CategoriesScrollView
{
    public class CategoriesScrollPresenter : ScrollPresenterBase<CategoriesItemModel, CategoriesItemView>
    {
        [Header("Snap")] 
        [SerializeField] [Range(0f, 20f)] 
        protected float snapSpeed = 10f;
        [SerializeField] 
        protected float snapThreshold = 0.5f;
        
        private DiContainer _diContainer;
        
        private float _lastDragDirection;
        private bool _shouldSnap;

        private TargetItemData _targetItemData;
        
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
        
        protected void LateUpdate()
        {
            if (_shouldSnap)
                SmoothSnap();
        }        
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            SaveViewsPosition();
            scrollRect.inertia = true;
            _shouldSnap = false;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            
            scrollRect.inertia = false;
            _lastDragDirection = Mathf.Sign(eventData.position.x - eventData.pressPosition.x);
            _targetItemData = FindNearestItemData();
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

        private TargetItemData FindNearestItemData()
        {
            var center = -content.anchoredPosition.x;
            var closestDist = float.MaxValue;
            var closestByDirectionDist = float.MaxValue;
            var closest = new TargetItemData();
            var closestByDirection = new TargetItemData();

            foreach (var item in ActiveItems)
            {
                var itemCenter = item.RectTransform.anchoredPosition.x - BorderSpacing;
                var distance = center - itemCenter;

                var validByDirection =
                    Mathf.Approximately(_lastDragDirection, 0f) ||
                    (_lastDragDirection > 0 && distance >= 0f) ||
                    (_lastDragDirection < 0 && distance <= 0f);


                var absDist = Mathf.Abs(distance);
                if (absDist < closestDist)
                {
                    closestDist = absDist;
                    closest.AnchoredPosition = item.RectTransform.anchoredPosition;
                    closest.ItemIndex = item.ItemIndex;
                }

                if (validByDirection && absDist < closestByDirectionDist)
                {
                    closestByDirectionDist = absDist;
                    closestByDirection.AnchoredPosition = item.RectTransform.anchoredPosition;
                    closestByDirection.ItemIndex = item.ItemIndex;
                }
            }

            return closestByDirectionDist < float.MaxValue ? closestByDirection : closest;
        }

        private void SaveViewsPosition()
        {
            foreach (var item in ActiveItems)
            {
                if (!item.Active) 
                    continue;
                
                var index = item.ItemIndex;
                Models[index].contentPosition = item.GetContentPosition();
            }
        }
    }
}