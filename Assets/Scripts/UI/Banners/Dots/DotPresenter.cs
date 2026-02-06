using System.Collections.Generic;
using UnityEngine;

namespace UI.Banners.Dots
{
    public class DotPresenter : MonoBehaviour
    {
        [SerializeField] private Dot dotPrefab;

        private readonly List<Dot> _dots = new();

        public void SpawnDots(int count, int activeIndex)
        {
            for (var i = 0; i < count; i++)
            {
                var dot = Instantiate(dotPrefab, transform);
                _dots.Add(dot);
            }

            SetDotActive(activeIndex);
        }

        public void SetDotActive(int index)
        {
            if (index < 0 || index >= _dots.Count)
                return;

            for (var i = 0; i < _dots.Count; i++) _dots[i].ChangeActive(i == index);
        }
    }
}