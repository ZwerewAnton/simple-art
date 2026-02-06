using PrimeTween;
using UI.Common.Dialog;
using UI.Mediators;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Premium
{
    public class PremiumDialog : CloseDialog
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private RectTransform roll;
        [SerializeField] private RectTransform mask;

        [Header("Animation")] [SerializeField] private float animationDuration = 0.5f;

        [SerializeField] private Vector2 rollStartPosition = new(0f, 502f);
        [SerializeField] private Vector2 rollEndPosition = new(0f, -2980);
        [SerializeField] private float maskOnHeight = 3040f;
        [SerializeField] private float maskOffHeight = 1f;

        private Sequence _animationSequence;

        private MainMenuMediator _mainMenuMediator;

        protected override void OnEnable()
        {
            base.OnEnable();
            continueButton.onClick.AddListener(PlayButtonClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            continueButton.onClick.RemoveListener(PlayButtonClick);
        }

        [Inject]
        private void Construct(MainMenuMediator mainMenuMediator)
        {
            _mainMenuMediator = mainMenuMediator;
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

            PlayButtonClick();
        }

        private void AnimateOpening()
        {
            if (_animationSequence.isAlive)
                return;

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
                return;

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

        private void PlayButtonClick()
        {
            _mainMenuMediator.PlayButtonClick();
        }
    }
}