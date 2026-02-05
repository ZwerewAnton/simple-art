using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Banners
{
    public class BannerScrollPresenter : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [Header("References")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform viewport;
        [SerializeField] private RectTransform content;

        [Header("Items")]
        [SerializeField] private List<GameObject> bannerPrefabs;

        [Header("Settings")]
        [SerializeField] private float snapSpeed = 10f;

        private List<RectTransform> _items = new();
        private float _itemSize;
        private int _centerIndex = 0;
        private bool _dragging = false;

        private void Start()
        {
            _itemSize = viewport.rect.width;
            SpawnBanners();
            LayoutItems();
        }

        private void Update()
        {
            if (_items.Count == 0) return;

            // 1. Перестановка элементов во время drag
            if (_dragging)
            {
                HandleDragShift();
            }

            // 2. Перемещение content и snap после остановки
            if (!_dragging && scrollRect.velocity.sqrMagnitude < 5f)
            {
                HandleStopped();
            }
        }

        private void SpawnBanners()
        {
            foreach (Transform child in content)
                Destroy(child.gameObject);

            _items.Clear();

            foreach (var prefab in bannerPrefabs)
            {
                var go = Instantiate(prefab, content);
                var rect = go.GetComponent<RectTransform>();
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(_itemSize, rect.sizeDelta.y);

                _items.Add(rect);
            }
        }

        private void LayoutItems()
        {
            int count = _items.Count;

            for (int i = 0; i < count; i++)
            {
                int relativeIndex = GetRelativeIndex(i, _centerIndex, count);
                float x = relativeIndex * _itemSize;
                _items[i].anchoredPosition = new Vector2(x, 0f);
            }
        }

        private int GetRelativeIndex(int itemIndex, int centerIndex, int count)
        {
            int diff = itemIndex - centerIndex;

            if (diff > count / 2)
                diff -= count;
            else if (diff < -count / 2)
                diff += count;

            return diff;
        }

        private void HandleDragShift()
        {
            // Проверяем каждый элемент
            float halfItem = _itemSize / 2f;
            int count = _items.Count;

            for (int i = 0; i < count; i++)
            {
                // Позиция элемента относительно центра viewport
                float localX = _items[i].localPosition.x + content.anchoredPosition.x;

                if (localX < -_itemSize) // ушёл влево
                {
                    _items[i].anchoredPosition += new Vector2(_itemSize * count, 0f);
                }
                else if (localX > _itemSize) // ушёл вправо
                {
                    _items[i].anchoredPosition -= new Vector2(_itemSize * count, 0f);
                }
            }
        }

        private void HandleStopped()
        {
            float x = content.anchoredPosition.x;

            if (Mathf.Abs(x) < _itemSize * 0.5f)
                return; // остаёмся на том же элементе

            int direction = x > 0 ? -1 : 1;

            MoveCenter(direction);
        }

        private void MoveCenter(int direction)
        {
            int count = _items.Count;
            _centerIndex = (_centerIndex + direction + count) % count;

            // Сбрасываем смещение content
            content.anchoredPosition = Vector2.zero;
            scrollRect.velocity = Vector2.zero;

            LayoutItems();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;
        }
    }
}
