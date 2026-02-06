using System;
using System.Collections.Generic;
using Device;
using UI.Common.ScrollView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace UI.Banners
{
    public class BannerScrollPresenter : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [Header("References")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform viewport;
        [SerializeField] private RectTransform content;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;

        [Header("Items")]
        [SerializeField] private List<GameObject> phoneBannerPrefabs;
        [SerializeField] private List<GameObject> tabletBannerPrefabs;

        [Header("Settings")]
        [SerializeField] private bool snap;
        [SerializeField] private float snapSpeed = 10f;
        [SerializeField] protected float snapThreshold = 0.5f;
        
        [Header("Auto Scroll")]
        [SerializeField] private bool autoScroll = true;
        [SerializeField] private float autoScrollDelay = 5f;

        public event Action<int> FocusItemChanged;
        public event Action Initialized;
        public int ItemsCount => _items.Count;
        public int FocusItemIndex { get; private set; }

        private readonly List<RectTransform> _items = new();

        private float _itemSize;
        private float _center;
        private bool _dragging = false;
        private float _lastShiftX;
        private int _lastCenterBannerIndex;
        
        private int _nextIndex;
        private int _prevIndex;

        private TargetItemData _targetItemData;
        private bool _shouldSnap;
        private float _lastDragDirection;
        private float _autoScrollTimer;
        
        private IDeviceService _deviceService;
        private DiContainer _diContainer;

        [Inject]
        private void Construct(IDeviceService deviceService, DiContainer diContainer)
        {
            _deviceService = deviceService;
            _diContainer = diContainer;
        }
        
        protected virtual void OnEnable()
        {
            if (scrollRect != null)
                scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }
        
        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _itemSize = viewport.rect.width;
            SpawnBanners();
            LayoutItems();
            
            FocusItemIndex = 0;
            _prevIndex = Prev(FocusItemIndex);
            _nextIndex = Next(FocusItemIndex);
            
            Initialized?.Invoke();
        }

        private void Update()
        {
            if (!autoScroll || _dragging || _shouldSnap)
                return;

            _autoScrollTimer += Time.deltaTime;

            if (_autoScrollTimer >= autoScrollDelay)
            {
                _autoScrollTimer = 0f;
                AutoScrollNext();
            }
        }

        protected void LateUpdate()
        {
            if (snap && _shouldSnap)
                SmoothSnap();
        }
        
        private void OnDisable()
        {
            if (scrollRect != null)
                scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
            _autoScrollTimer = 0f;
            
            DisableSnap();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;
            _autoScrollTimer = 0f;
            
            if (snap)
            {
                _lastDragDirection = Mathf.Sign(eventData.position.x - eventData.pressPosition.x);
                _targetItemData = FindNearestItemData();
                EnableSnap();
            }
        }
        
        private void MoveCenter()
        {
            content.anchoredPosition = Vector2.zero;
            scrollRect.velocity = Vector2.zero;
            _lastShiftX = 0f;

            LayoutItems();
        }
        
        private void LayoutItems()
        {
            var count = _items.Count;

            for (var i = 0; i < count; i++)
            {
                var relativeIndex = GetRelativeIndex(i, FocusItemIndex, count);
                var x = relativeIndex * _itemSize;

                _items[i].anchoredPosition = new Vector2(x, 0f);
            }

            content.anchoredPosition = Vector2.zero;
        }
        
        private static int GetRelativeIndex(int itemIndex, int centerIndex, int count)
        {
            var diff = itemIndex - centerIndex;

            if (diff > count / 2)
                diff -= count;
            else if (diff < -count / 2)
                diff += count;

            return diff;
        }
        
        private void OnScrollChanged(Vector2 _)
        {
            var currentX = content.anchoredPosition.x;
            var delta = currentX - _lastShiftX;
            
            var threshold = _itemSize * 0.5f;

            if (delta <= -threshold)
            {
                var centerItemPosition = _items[_nextIndex].anchoredPosition.x;
                var newBannerPosition =  _itemSize + centerItemPosition;
                _items[_prevIndex].anchoredPosition = new Vector2(newBannerPosition, 0f);
                
                FocusItemIndex = _nextIndex;
                _nextIndex = Next(FocusItemIndex);
                _prevIndex = Prev(FocusItemIndex);
                
                _lastShiftX -= _itemSize;
                
                FocusItemChanged?.Invoke(FocusItemIndex);
            }
            else if (delta >= threshold)
            {
                var centerX = _items[_prevIndex].anchoredPosition.x;
                var newX = centerX - _itemSize;

                _items[_nextIndex].anchoredPosition = new Vector2(newX, 0f);

                FocusItemIndex = _prevIndex;
                _nextIndex = Next(FocusItemIndex);
                _prevIndex = Prev(FocusItemIndex);
                
                _lastShiftX += _itemSize;
                
                FocusItemChanged?.Invoke(FocusItemIndex);
            }
        }

        private int Prev(int index)
        {
            return (index - 1 + _items.Count) % _items.Count;
        }
        
        private int Next(int index)
        {
            return (index + 1) % _items.Count;
        }
        
        private void AutoScrollNext()
        {
            _lastDragDirection = -1f;
            _targetItemData = new TargetItemData
            {
                ItemIndex = _nextIndex,
                AnchoredPosition = _items[_nextIndex].anchoredPosition
            };

            EnableSnap();
        }
        
        protected virtual void SmoothSnap()
        {
            var currentX = content.anchoredPosition.x;
            var targetX = -_targetItemData.AnchoredPosition.x;

            var newX = Mathf.Lerp(currentX, targetX, snapSpeed * Time.deltaTime);
            content.anchoredPosition = new Vector2(newX, content.anchoredPosition.y);

            if (Mathf.Abs(newX - targetX) < snapThreshold)
            {
                content.anchoredPosition = new Vector2(targetX, content.anchoredPosition.y);
                _shouldSnap = false;
                FocusItemChanged?.Invoke(FocusItemIndex);
                MoveCenter();
            }
        }
        
        private TargetItemData FindNearestItemData()
        {
            var center = -content.anchoredPosition.x;
            var closestDist = float.MaxValue;
            var closestByDirectionDist = float.MaxValue;
            var closest = new TargetItemData();
            var closestByDirection = new TargetItemData();

            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var itemCenter = item.anchoredPosition.x;
                var distance = center - itemCenter;

                var validByDirection =
                    Mathf.Approximately(_lastDragDirection, 0f) ||
                    (_lastDragDirection > 0 && distance >= 0f) ||
                    (_lastDragDirection < 0 && distance <= 0f);


                var absDist = Mathf.Abs(distance);
                if (absDist < closestDist)
                {
                    closestDist = absDist;
                    closest.AnchoredPosition = item.anchoredPosition;
                    closest.ItemIndex = i;
                }

                if (validByDirection && absDist < closestByDirectionDist)
                {
                    closestByDirectionDist = absDist;
                    closestByDirection.AnchoredPosition = item.anchoredPosition;
                    closestByDirection.ItemIndex = i;
                }
            }

            return closestByDirectionDist < float.MaxValue ? closestByDirection : closest;
        }

        private void SpawnBanners()
        {
            var prefabs = _deviceService.Device == Device.Device.Phone ? phoneBannerPrefabs : tabletBannerPrefabs;
            foreach (var t in prefabs)
            {
                var go = _diContainer.InstantiatePrefab(t, content);
                var itemRect = go.GetComponent<RectTransform>();
                _items.Add(itemRect);
            }
        }
        
        private void DisableSnap()
        {
            _shouldSnap = false;
            scrollRect.inertia = true;
        }

        private void EnableSnap()
        {
            scrollRect.inertia = false;
            _shouldSnap = true;
        }
    }
}
