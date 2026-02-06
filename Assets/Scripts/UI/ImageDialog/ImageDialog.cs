using System;
using PrimeTween;
using UI.Common.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ImageDialog
{
    public class ImageDialog : CloseDialog
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RawImage rawImage;
        [Header("Animation")]
        [SerializeField] private float animationDuration = 0.5f;
        
        private Tween _animationTween;

        private void Awake()
        {
            rawImage.rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x);
        }

        public void Show(Texture2D image)
        {
            Show();
            rawImage.texture = image;
        }
        
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
            if (_animationTween.isAlive)
                _animationTween.Stop();

            canvasGroup.alpha = 0;

            _animationTween = Tween.Alpha(canvasGroup,
                1f,
                animationDuration,
                Ease.InOutSine
            );
        }

        private void AnimateClosing()
        {
            if (_animationTween.isAlive)
                _animationTween.Stop();

            canvasGroup.alpha = 1;

            _animationTween = Tween.Alpha(canvasGroup,
                0f,
                animationDuration,
                Ease.InOutSine
            ).OnComplete(() => gameObject.SetActive(false));
        }
    }
}