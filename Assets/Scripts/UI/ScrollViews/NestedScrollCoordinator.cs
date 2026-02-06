using System;
using UI.Categories.CategoriesScrollView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.ScrollViews
{
    public class NestedScrollCoordinator : MonoBehaviour
    {
        private const float Threshold = 10f;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private CategoriesScrollPresenter categoriesScrollPresenter;
        [SerializeField] private ScrollRect scrollRect;

        private ScrollInputMode _currentMode = ScrollInputMode.None;
        private Vector2 _startPosition;

        public event Action NestedScrollBlocked;
        public event Action NestedScrollUnblocked;

        public void OnNestedBeginDrag(PointerEventData eventData)
        {
            _startPosition = eventData.position;
            _currentMode = ScrollInputMode.None;
            NestedScrollBlocked?.Invoke();
        }

        public void OnNestedDrag(PointerEventData eventData)
        {
            if (_currentMode == ScrollInputMode.None)
            {
                DetermineMode(eventData);

                if (_currentMode == ScrollInputMode.Horizontal)
                {
                    categoriesScrollPresenter.OnBeginDrag(eventData);
                    scrollRect.OnBeginDrag(eventData);
                }

                if (_currentMode == ScrollInputMode.Vertical) NestedScrollUnblocked?.Invoke();
            }

            if (_currentMode == ScrollInputMode.Horizontal)
            {
                categoriesScrollPresenter.OnDrag(eventData);
                scrollRect.OnDrag(eventData);
            }
        }

        public void OnNestedEndDrag(PointerEventData eventData)
        {
            if (_currentMode == ScrollInputMode.Horizontal)
            {
                categoriesScrollPresenter.OnEndDrag(eventData);
                scrollRect.OnEndDrag(eventData);

                NestedScrollUnblocked?.Invoke();
            }

            _currentMode = ScrollInputMode.None;
        }

        private void DetermineMode(PointerEventData eventData)
        {
            var delta = eventData.position - _startPosition;

            if (delta.magnitude < Threshold)
                return;

            _currentMode = Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
                ? ScrollInputMode.Horizontal
                : ScrollInputMode.Vertical;
        }
    }
}