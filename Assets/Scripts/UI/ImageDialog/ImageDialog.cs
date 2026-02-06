using System;
using PrimeTween;
using UI.Common.Dialog;
using UI.Mediators;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.ImageDialog
{
    public class ImageDialog : CloseDialog
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RawImage rawImage;
        [Header("Animation")]
        [SerializeField] private float animationDuration = 0.5f;
        
        private Tween _animationTween;
        
        private MainMenuMediator _mainMenuMediator;

        [Inject]
        private void Construct(MainMenuMediator mainMenuMediator)
        {
            _mainMenuMediator = mainMenuMediator;
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

        protected override void Close()
        {
            base.Close();
            
            _mainMenuMediator.PlayButtonClick();
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
                _animationTween.Complete();

            canvasGroup.alpha = 1;

            _animationTween = Tween.Alpha(canvasGroup,
                0f,
                animationDuration,
                Ease.InOutSine
            ).OnComplete(() => gameObject.SetActive(false));
        }
    }
}