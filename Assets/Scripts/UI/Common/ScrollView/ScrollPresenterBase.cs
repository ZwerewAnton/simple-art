using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Common.ScrollView
{
    public class ScrollPresenterBase<TModel, TItem> : MonoBehaviour, IBeginDragHandler, IDragHandler,
        IEndDragHandler
        where TItem : ScrollItemView<TModel>
    {
        [SerializeField] protected ScrollRect scrollRect;
        [SerializeField] protected RectTransform content;
        [SerializeField] protected GameObject itemPrefab;

        [Header("Items")] 
        [Range(0f, 500f)] [SerializeField] protected float itemSpacing = 50f;
        [Range(0f, 500f)] [SerializeField] protected float borderSpacing = 50f;
        [Range(0f, 10f)] [SerializeField] protected int additionalPoolItemsCount = 2;
        
        protected readonly List<TItem> ActiveItems = new();
        protected readonly List<TModel> Models = new();
        protected float BorderSpacing;
        protected bool Initialized;
        protected float ItemSize;
        protected int ViewItemCount;
        
        private bool _markToUpdate;

        #region Unity Events

        protected virtual void OnEnable()
        {
            if (scrollRect != null)
                scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }

        protected void Update()
        {
            UpdateScroll();
        }

        protected virtual void OnDisable()
        {
            if (scrollRect != null)
                scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
        }

        protected virtual void OnDestroy()
        {
            ClearPool();
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
        }

        #endregion

        #region Initialization

        public virtual void Initialize(List<TModel> newModels)
        {
            if (newModels == null || newModels.Count == 0)
                return;

            if (Initialized)
                ClearPool();

            Initialized = true;

            Models.Clear();
            Models.AddRange(newModels);

            var itemRect = itemPrefab.GetComponent<RectTransform>();
            ItemSize = CalculateItemSize(itemRect);
            BorderSpacing = GetBorderSpacing();

            SetContentSize();
            var cellSize = ItemSize + itemSpacing;
            ViewItemCount = CalculateViewItemCount(cellSize);

            CreatePool();
            UpdateVisibleItems();
        }

        protected virtual void SetContentSize()
        {
            var size = (ItemSize + itemSpacing) * Models.Count - itemSpacing + BorderSpacing * 2;
            content.sizeDelta = scrollRect.horizontal
                ? new Vector2(size, content.sizeDelta.y)
                : new Vector2(content.sizeDelta.x, size);
        }

        protected virtual int CalculateViewItemCount(float cellSize)
        {
            return Mathf.Min(Models.Count, Mathf.CeilToInt(GetViewportSize() / cellSize)) + additionalPoolItemsCount;
        }
        
        protected virtual void SetupItemRectTransform(RectTransform rect)
        {
            if (scrollRect.horizontal)
            {
                rect.anchorMin = new Vector2(0f, 0.5f);
                rect.anchorMax = new Vector2(0f, 0.5f);
                rect.pivot = new Vector2(0f, 0.5f);
            }
            else
            {
                rect.anchorMin = new Vector2(0.5f, 1f);
                rect.anchorMax = new Vector2(0.5f, 1f);
                rect.pivot = new Vector2(0.5f, 1f);
            }
        }

        #endregion

        #region Pool Management

        protected virtual void CreatePool()
        {
            ClearPool();

            for (var i = 0; i < ViewItemCount; i++)
            {
                var go = Instantiate(itemPrefab, content);
                var item = go.GetComponent<TItem>();
                SetupItemRectTransform(item.RectTransform);
                item.Clicked += OnItemClicked;
                ActiveItems.Add(item);
            }
        }

        protected virtual void ClearPool()
        {
            foreach (var item in ActiveItems)
            {
                if (item == null)
                    continue;

                item.Clicked -= OnItemClicked;
                Destroy(item.gameObject);
            }

            ActiveItems.Clear();
        }

        #endregion

        #region Content Management

        public virtual void UpdateModels(List<TModel> models)
        {
            Models.Clear();
            Models.AddRange(models);

            //MarkToUpdate();
        }

        protected virtual void UpdateScroll()
        {
            if (!_markToUpdate)
                return;

            UpdateVisibleItems();
            _markToUpdate = false;
        }

        protected virtual void OnScrollChanged(Vector2 _)
        {
            UpdateVisibleItems();
        }

        protected virtual void UpdateVisibleItems()
        {
            if (!Initialized)
                return;

            var offset = GetScrollOffset();
            var cellSize = ItemSize + itemSpacing;
            var firstVisibleIndex = Mathf.FloorToInt((offset - borderSpacing) / cellSize);
            firstVisibleIndex = Mathf.Max(0, firstVisibleIndex);

            var viewportSize = GetViewportSize();
            var lastVisibleIndex =
                Mathf.CeilToInt((offset - borderSpacing - cellSize + viewportSize) / cellSize);
            
            for (var i = 0; i < ActiveItems.Count; i++)
            {
                var modelIndex = firstVisibleIndex + i;
                var item = ActiveItems[i];

                if (modelIndex < firstVisibleIndex || modelIndex > lastVisibleIndex || modelIndex >= Models.Count)
                {
                    item.gameObject.SetActive(false);
                    continue;
                }

                if (!item.gameObject.activeSelf)
                    item.gameObject.SetActive(true);

                item.SetData(modelIndex, Models[modelIndex]);
                item.RectTransform.anchoredPosition = GetAnchoredPosition(modelIndex);
            }
        }

        protected virtual void OnItemClicked(int itemIndex)
        {
        }

        protected virtual float CalculateItemSize(RectTransform rect)
        {
            return scrollRect.horizontal
                ? rect.rect.width
                : rect.rect.height;
        }

        protected virtual float GetViewportSize()
        {
            return scrollRect.horizontal
                ? scrollRect.viewport.rect.width
                : scrollRect.viewport.rect.height;
        }

        protected virtual float GetBorderSpacing()
        {
            return borderSpacing;
        }

        protected virtual Vector2 GetAnchoredPosition(int index)
        {
            var position = index * (ItemSize + itemSpacing) + BorderSpacing;
            return scrollRect.horizontal
                ? new Vector2(position, 0f)
                : new Vector2(0f, -position);
        }

        protected virtual float GetScrollOffset()
        {
            return scrollRect.horizontal
                ? -content.anchoredPosition.x
                : content.anchoredPosition.y;
        }

        protected virtual float GetItemsOffset()
        {
            var cellSize = ItemSize + itemSpacing;
            return GetScrollOffset() - cellSize;
        }

        protected void MarkToUpdate()
        {
            _markToUpdate = true;
        }

        #endregion
    }
}