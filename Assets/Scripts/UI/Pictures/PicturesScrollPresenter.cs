using System.Collections.Generic;
using System.Linq;
using Device;
using UI.Common.ScrollView;
using UI.Mediators;
using UI.ScrollViews;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.Pictures
{
    public class PicturesScrollPresenter : ScrollPresenterBase<PictureItemModel[], PictureGridItemView>
    {
        [Header("Grid")] 
        [SerializeField] protected float picture2CellSize = 640f;
        [SerializeField] protected float picture3CellSize = 413f;
        [SerializeField] protected float cellSpacing = 40f;
        [SerializeField] protected float horizontalPadding = 60f;

        private int _itemsPerRow = 2; 

        private NestedScrollCoordinator _coordinator;
        private DiContainer _diContainer;
        private MainMenuMediator _mainMenuMediator;
        private IDeviceService _deviceService;

        [Inject]
        private void Construct(
            NestedScrollCoordinator scrollCoordinator, 
            DiContainer diContainer,
            MainMenuMediator mainMenuMediator,
            IDeviceService deviceService)
        {
            _coordinator = scrollCoordinator;
            _diContainer = diContainer;
            _mainMenuMediator = mainMenuMediator;
            _deviceService = deviceService;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _coordinator.NestedScrollBlocked += BlockScroll;
            _coordinator.NestedScrollUnblocked += UnblockScroll;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            _coordinator.NestedScrollBlocked -= BlockScroll;
            _coordinator.NestedScrollUnblocked -= UnblockScroll;
        }

        public void Initialize()
        {
            if (IsInitialized)
                ClearPool();
            
            IsInitialized = true;

            Models.Clear();

            var itemRect = itemPrefab.GetComponent<RectTransform>();
            _itemsPerRow = _deviceService.Device == Device.Device.Phone ? 2 : 3;
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

        protected override float CalculateItemSize(RectTransform rect)
        {
            return _itemsPerRow == 3 ? picture3CellSize : picture2CellSize;
        }

        protected override int CalculateViewItemCount(float cellSize)
        {
            return Mathf.CeilToInt(GetViewportSize() / cellSize) + additionalPoolItemsCount;
        }
        
        protected override void CreatePool()
        {
            ClearPool();

            for (var i = 0; i < ViewItemCount; i++)
            {
                var go = _diContainer.InstantiatePrefab(itemPrefab, content);
                var item = go.GetComponent<PictureGridItemView>();
                SetupItemRectTransform(item.RectTransform);
                item.CellClicked += OnCellItemClicked;
                ActiveItems.Add(item);
            }
            
            foreach (var gridItemView in ActiveItems)
            {
                gridItemView.Initialize(_itemsPerRow,  ItemSize, cellSpacing, horizontalPadding);
            }
        }

        protected override void ClearPool()
        {
            base.ClearPool();

            foreach (var gridItemView in ActiveItems)
            {
                gridItemView.CellClicked -= OnCellItemClicked;
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
        
        public Vector2 GetContentPosition()
        {
            return scrollRect.content.anchoredPosition;
        }

        public void SetContentPosition(Vector2 position)
        {
            scrollRect.content.anchoredPosition = position;
        }

        private void OnCellItemClicked(int itemIndex, int cellItemIndex)
        {
            base.OnItemClicked(itemIndex);
            
            _mainMenuMediator.PlayButtonClick();
            
            var item = ActiveItems.FirstOrDefault(i => i.ItemIndex == itemIndex);
            if (item == null)
                return;

            var index = item.ItemIndex;
            if (index < 0 || index >= Models.Count || cellItemIndex  < 0 || cellItemIndex >= _itemsPerRow)
                return;
            
            var model = Models[index][cellItemIndex];

            if (model.IsPremium)
            {
                _mainMenuMediator.ShowPremiumDialog();
            }
            else
            {
                _mainMenuMediator.ShowImageDialog(model.ImageUrl);
            }
        }

        private List<PictureItemModel[]> BuildRows(List<PictureItemModel> models)
        {
            var rows = new List<PictureItemModel[]>();

            for (var i = 0; i < models.Count; i += _itemsPerRow)
            {
                var count = Mathf.Min(_itemsPerRow, models.Count - i);
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
            scrollRect.inertia = false;
            scrollRect.vertical = false;
        }

        private void UnblockScroll()
        {
            scrollRect.velocity = Vector2.zero;
            scrollRect.vertical = true;
            scrollRect.inertia = true;
        }
    }
}