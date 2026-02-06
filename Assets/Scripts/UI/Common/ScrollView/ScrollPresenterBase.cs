using System;
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

        [Header("Snap")] 
        [SerializeField] protected bool snap;
        [SerializeField] [Range(0f, 20f)] protected float snapSpeed = 10f;
        [SerializeField] protected float snapThreshold = 0.5f;
        
        public event Action Initialized;
        public float ItemSize { get; protected set; }
        public int ModelsCount => Models.Count;
        
        protected readonly List<TItem> ActiveItems = new();
        protected readonly List<TModel> Models = new();
        protected float BorderSpacing;
        protected bool IsInitialized;
        protected int ViewItemCount;
        private float LastDragDirection;
        
        protected TargetItemData TargetItemData;
        protected bool ShouldSnap;
        
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
        
        protected void LateUpdate()
        {
            if (snap && ShouldSnap)
                SmoothSnap();
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
            DisableSnap();
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (snap)
            {
                LastDragDirection = Mathf.Sign(eventData.position.x - eventData.pressPosition.x);
                TargetItemData = FindNearestItemData();
                EnableSnap();
            }
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

            if (IsInitialized)
                ClearPool();

            IsInitialized = true;

            Models.Clear();
            Models.AddRange(newModels);

            var itemRect = itemPrefab.GetComponent<RectTransform>();
            ItemSize = CalculateItemSize(itemRect);
            BorderSpacing = GetBorderSpacing();

            SetContentSize();
            var cellSize = ItemSize + itemSpacing;
            ViewItemCount = CalculateViewItemCount(cellSize);

            CreatePool();
            
            Initialized?.Invoke();
            
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
            if (!IsInitialized)
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
                    item.SetActive(false);
                    continue;
                }

                item.SetActive(true);

                if (item.ItemIndex != modelIndex || !item.IsRefreshed)
                {
                    item.SetData(modelIndex, Models[modelIndex]);
                }

                item.SetAnchoredPosition(GetAnchoredPosition(modelIndex));
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

        protected virtual void OnItemDeactivate(TItem item)
        {
        }

        protected void MarkViewsToRefresh()
        {
            foreach (var itemView in ActiveItems)
            {
                itemView.IsRefreshed = false;
            }
        }

        protected void MarkToUpdate()
        {
            _markToUpdate = true;
        }

        #endregion

        #region Animation

        protected virtual void DisableSnap()
        {
            ShouldSnap = false;
            scrollRect.inertia = true;
        }

        protected virtual void EnableSnap()
        {
            scrollRect.inertia = false;
            ShouldSnap = true;
        }

        protected virtual void SmoothSnap()
        {
            var currentX = content.anchoredPosition.x;
            var targetX = -TargetItemData.AnchoredPosition.x + BorderSpacing;

            var newX = Mathf.Lerp(currentX, targetX, snapSpeed * Time.deltaTime);
            content.anchoredPosition = new Vector2(newX, content.anchoredPosition.y);

            if (Mathf.Abs(newX - targetX) < snapThreshold)
            {
                content.anchoredPosition = new Vector2(targetX, content.anchoredPosition.y);
                ShouldSnap = false;
                OnSnapEnded();
            }
        }

        protected virtual void OnSnapEnded()
        {
            
        }
        
        protected virtual TargetItemData FindNearestItemData()
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
                    Mathf.Approximately(LastDragDirection, 0f) ||
                    (LastDragDirection > 0 && distance >= 0f) ||
                    (LastDragDirection < 0 && distance <= 0f);


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

        #endregion
    }
}