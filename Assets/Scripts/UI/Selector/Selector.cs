using System.Collections.Generic;
using UnityEngine;

namespace UI.Selector
{
    public class Selector : MonoBehaviour
    {
        [Header("Items")]
        [SerializeField] private List<SelectorItem> items;
        [SerializeField] private int startElement;
        
        [Header("Animation")]
        [SerializeField] private float animationTime = 0.3f;
        
        private SelectorItem _current;

        private void Awake()
        {
            foreach (var item in items)
                item.Clicked += OnItemClicked;
        }

        private void OnDestroy()
        {
            foreach (var item in items)
                item.Clicked -= OnItemClicked;
        }

        private void Start()
        {
            if (startElement < 0 && startElement >= items.Count)
                return;
            
            for (var i = 0; i < items.Count; i++)
            {
                items[i].SetActiveImmediately(startElement == i);
                if (startElement == i)
                    _current = items[i];
            }
        }

        private void OnItemClicked(SelectorItem item)
        {
            Select(item);
        }

        private void Select(SelectorItem item)
        {
            if (_current == item)
                return;

            if (_current != null)
                _current.SetActive(false, animationTime);

            _current = item;
            _current.SetActive(true, animationTime);
        }
    }
}