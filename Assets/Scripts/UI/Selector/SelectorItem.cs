using System;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Selector
{
    public class SelectorItem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image activeIcon;
        [SerializeField] private TMP_Text text;
        [SerializeField] private TMP_Text subText;
        [SerializeField] private Button button;

        [Header("Colors")]
        [SerializeField] private Color activeTextColor = Color.red;
        [SerializeField] private Color inactiveTextColor = Color.green;
        [SerializeField] private Color activeSubtextColor = Color.red;
        [SerializeField] private Color inactiveSubtextColor = Color.green;
        
        public bool IsActive { get; private set; }

        public event Action<SelectorItem> Clicked;
        
        private Sequence _sequence;
        private bool _subTextActive;

        private void Awake()
        {
            button.onClick.AddListener(OnClick);
            _subTextActive = subText != null;
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            Clicked?.Invoke(this);
        }

        public void SetActive(bool active, float time)
        {
            if (IsActive == active)
                return;
            
            if (_sequence.isAlive)
                _sequence.Stop();

            var sequence = Sequence.Create()
                .Group(Tween.Alpha(activeIcon, active ? 1f : 0f, time))
                .Group(Tween.Color(text, active ? activeTextColor : inactiveTextColor, time));
                
            if (_subTextActive)
                sequence.Group(Tween.Color(subText, active ? activeSubtextColor : inactiveSubtextColor, time));

            IsActive = active;
        }

        public void SetActiveImmediately(bool active)
        {
            if (_sequence.isAlive)
                _sequence.Stop();
            
            IsActive = active;

            var color = activeIcon.color;
            color.a = active ? 1f : 0f;
            activeIcon.color = color;
            text.color = active ? activeTextColor : inactiveTextColor;
            if (_subTextActive)
                subText.color = active ? activeSubtextColor : inactiveSubtextColor;
        }
    }
}