using PrimeTween;
using UI.Common.Dialog;
using UnityEngine;

namespace UI.Premium
{
    public class PremiumDialog : CloseDialog
    {
        [SerializeField] private RectTransform roll;
        [SerializeField] private RectTransform mask;
        [Header("Animation")]
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private Vector2 rollStartPosition = new (0f, 502f);
        [SerializeField] private Vector2 rollEndPosition = new (0f, -2980);
        [SerializeField] private float maskOnHeight = 3040f;
        [SerializeField] private float maskOffHeight = 1f;

        private Sequence _animationSequence;

        public override void Show()
        {
            base.Show();
            AnimateOpening();
        }

        public override void Hide()
        {
            AnimateClosing();
        }
        
        private void AnimateOpening()
        {
            if (_animationSequence.isAlive)
                _animationSequence.Stop();
            
            roll.gameObject.SetActive(true);
            _animationSequence = Sequence.Create();
            _animationSequence.Group(
                Tween.UIAnchoredPosition(
                    roll,
                    rollStartPosition,
                    rollEndPosition,
                    animationDuration,
                    Ease.InOutSine
                )
            );
            _animationSequence.Group(
                Tween.UISizeDelta(
                    mask,
                    new Vector2(mask.sizeDelta.x, maskOffHeight),
                    new Vector2(mask.sizeDelta.x, maskOnHeight),
                    animationDuration,
                    Ease.InOutSine
                )
            );
        }
        
        private void AnimateClosing()
        {
            if (_animationSequence.isAlive)
                _animationSequence.Stop();
            
            roll.gameObject.SetActive(true);
            _animationSequence = Sequence.Create();
            _animationSequence.Group(
                Tween.UIAnchoredPosition(
                    roll,
                    rollEndPosition,
                    rollStartPosition,
                    animationDuration,
                    Ease.InOutSine
                )
            );
            _animationSequence.Group(
                Tween.UISizeDelta(
                    mask,
                    new Vector2(mask.sizeDelta.x, maskOnHeight),
                    new Vector2(mask.sizeDelta.x, maskOffHeight),
                    animationDuration,
                    Ease.InOutSine
                )
            );
        }
    }
}