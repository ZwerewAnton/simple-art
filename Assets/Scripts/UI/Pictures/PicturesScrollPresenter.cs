using System;
using System.Collections.Generic;
using UI.Common.ScrollView;
using UI.ScrollViews;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.Pictures
{
    public class PicturesScrollPresenter : ScrollPresenterBase<PictureItemModel[], PictureGridItemView>
    {
        [Header("Grid")] 
        [Range(2, 3)] [SerializeField] protected int itemsPerRow = 2;
        [SerializeField] protected float pictureCellSize = 640f;
        [SerializeField] protected float cellSpacing = 40f;
        [SerializeField] protected float horizontalPadding = 60f;

        private NestedScrollCoordinator _coordinator;

        [Inject]
        private void Construct(NestedScrollCoordinator scrollCoordinator)
        {
            _coordinator = scrollCoordinator;
        }

        protected void Start()
        {
            _coordinator.NestedScrollBlocked += BlockScroll;
            _coordinator.NestedScrollUnblocked += UnblockScroll;
        }

        protected override void OnDestroy()
        {
            _coordinator.NestedScrollBlocked -= BlockScroll;
            _coordinator.NestedScrollUnblocked -= UnblockScroll;
        }

        public void Initialize()
        {
            if (Initialized)
                ClearPool();

            Initialized = true;

            Models.Clear();

            var itemRect = itemPrefab.GetComponent<RectTransform>();
            ItemSize = CalculateItemSize(itemRect);
            BorderSpacing = GetBorderSpacing();
            
            var cellSize = ItemSize + itemSpacing;
            ViewItemCount = CalculateViewItemCount(cellSize);

            CreatePool();
        }
        
        public void Initialize(List<PictureItemModel> models)
        {
            base.Initialize(BuildRows(models));
        }
        
        protected override int CalculateViewItemCount(float cellSize)
        {
            return Mathf.CeilToInt(GetViewportSize() / cellSize) + additionalPoolItemsCount;
        }
        
        protected override void CreatePool()
        {
            base.CreatePool();

            foreach (var gridItemView in ActiveItems)
            {
                gridItemView.Initialize(itemsPerRow,  pictureCellSize, cellSpacing, horizontalPadding);
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            
            _coordinator.OnNestedBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            
            _coordinator.OnNestedDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            
            _coordinator.OnNestedEndDrag(eventData);
        }

        public void UpdateModels(List<PictureItemModel> models)
        {
            base.UpdateModels(BuildRows(models));
            
            SetContentSize();
            var cellSize = ItemSize + itemSpacing;
            ViewItemCount = CalculateViewItemCount(cellSize);
            
            UpdateVisibleItems();
        }

        private List<PictureItemModel[]> BuildRows(List<PictureItemModel> models)
        {
            var rows = new List<PictureItemModel[]>();

            for (var i = 0; i < models.Count; i += itemsPerRow)
            {
                var count = Mathf.Min(itemsPerRow, models.Count - i);
                var row = new PictureItemModel[count];

                for (var j = 0; j < count; j++)
                    row[j] = models[i + j];

                rows.Add(row);
            }

            return rows;
        }

        private void BlockScroll()
        {
            scrollRect.velocity = Vector2.zero;
            scrollRect.vertical = false;
        }

        private void UnblockScroll()
        {
            scrollRect.vertical = true;
        }
    }
}