using PrimeTween;
using UI.Common.Dialog;
using UnityEngine;

namespace UI.Premium
{
    public class PremiumDialog : CloseDialog
    {
        [Header("CloseAnimation")]
        [SerializeField] private RectTransform roll;
        [SerializeField] private RectTransform mask;
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
                    1f,
                    Ease.InOutSine
                )
            );
            _animationSequence.Group(
                Tween.UISizeDelta(
                    mask,
                    new Vector2(mask.sizeDelta.x, maskOffHeight),
                    new Vector2(mask.sizeDelta.x, maskOnHeight),
                    1f,
                    Ease.InOutSine
                )
            );
            _animationSequence.OnComplete(() => gameObject.SetActive(false));
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
                    1f,
                    Ease.InOutSine
                )
            );
            _animationSequence.Group(
                Tween.UISizeDelta(
                    mask,
                    new Vector2(mask.sizeDelta.x, maskOnHeight),
                    new Vector2(mask.sizeDelta.x, maskOffHeight),
                    1f,
                    Ease.InOutSine
                )
            );
        }
    }
}