using Coffee.UIExtensions;
using PrimeTween;
using UI.Mediators;
using UnityEngine;
using Zenject;
using Button = UnityEngine.UI.Button;

namespace UI.Banners
{
    public class RateUsBanner : BannerView
    {
        [Header("Elements")] [SerializeField] private RectTransform[] lights;

        [SerializeField] private RectTransform rays;
        [SerializeField] private RectTransform movingFrame;
        [SerializeField] private Button button;
        [SerializeField] private RectTransform bigStar;
        [SerializeField] private RectTransform smallStar;
        [SerializeField] private UIParticle uiParticle;

        [Header("Lights")] [SerializeField] private float lightScaleDelta = 0.08f;

        [SerializeField] private float lightDuration = 1.2f;

        [Header("Rays")] [SerializeField] private float raysAngle = 50f;

        [SerializeField] private float raysRotationDuration = 12f;

        [Header("Frame")] [SerializeField] private float frameAngle = 2.5f;

        [SerializeField] private float frameDuration = 3f;

        [Header("Stars")] [Header("Button")] [SerializeField]
        private float minScale = 0.6f;

        [SerializeField] private float pressDuration = 0.2f;
        [SerializeField] private float buttonAngle = 10f;
        [SerializeField] private float buttonRotationDuration = 3f;
        private Tween _bigStarTween;
        private Sequence _buttonPressSequence;
        private Tween _buttonRotateTween;
        private Tween _frameTween;

        private Sequence _lightSequence;

        private MainMenuMediator _mainMenuMediator;
        private Tween _rayTween;

        protected override void OnEnable()
        {
            base.OnEnable();

            button.onClick.AddListener(PlayButtonClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            button.onClick.RemoveListener(PlayButtonClick);
        }

        [Inject]
        private void Construct(MainMenuMediator mainMenuMediator)
        {
            _mainMenuMediator = mainMenuMediator;
        }

        protected override void StartAnimation()
        {
            StopAnimation();

            uiParticle.Play();
            AnimateLights();
            AnimateRays();
            AnimateFrame();
            AnimateButton();
            AnimateStars();
        }

        protected override void StopAnimation()
        {
            if (_lightSequence.isAlive)
                _lightSequence.Stop();
            if (_buttonPressSequence.isAlive)
                _buttonPressSequence.Stop();
            if (_frameTween.isAlive)
                _frameTween.Stop();
            if (_rayTween.isAlive)
                _rayTween.Stop();
            if (_bigStarTween.isAlive)
                _bigStarTween.Stop();
            if (_buttonRotateTween.isAlive)
                _buttonRotateTween.Stop();
            uiParticle.Stop();
        }

        private void AnimateStars()
        {
        }

        private void AnimateButton()
        {
            _buttonPressSequence = Sequence.Create(-1);

            _buttonPressSequence.ChainDelay(2f);
            _buttonPressSequence.Chain(Tween.Scale(button.transform, Vector3.one * minScale, pressDuration,
                Ease.OutBack));
            _buttonPressSequence.Chain(Tween.Scale(button.transform, Vector3.one, pressDuration, Ease.OutBack));
            _buttonRotateTween = Tween.LocalRotation(
                button.transform,
                new Vector3(0, 0, buttonAngle),
                new Vector3(0, 0, -buttonAngle),
                buttonRotationDuration,
                Ease.InOutSine,
                -1,
                CycleMode.Yoyo
            );
        }

        private void AnimateLights()
        {
            _lightSequence = Sequence.Create(-1, Sequence.SequenceCycleMode.Yoyo);

            for (var i = 0; i < lights.Length; i++)
            {
                var lightRect = lights[i];
                var baseScale = lightRect.localScale;

                var inverted = i % 2 == 1;
                var from = inverted ? 1f - lightScaleDelta : 1f + lightScaleDelta;
                var to = inverted ? 1f + lightScaleDelta : 1f - lightScaleDelta;

                _lightSequence.Group(
                    Tween.Scale(
                        lightRect,
                        baseScale * from,
                        baseScale * to,
                        lightDuration,
                        Ease.InOutSine
                    )
                );
            }
        }

        private void AnimateRays()
        {
            _rayTween = Tween.LocalRotation(
                rays,
                new Vector3(0, 0, -raysAngle),
                new Vector3(0, 0, raysAngle),
                raysRotationDuration,
                Ease.InOutSine,
                -1,
                CycleMode.Yoyo
            );
        }

        private void AnimateFrame()
        {
            _frameTween = Tween.LocalRotation(
                movingFrame,
                new Vector3(0, 0, -frameAngle),
                new Vector3(0, 0, frameAngle),
                frameDuration,
                Ease.InOutSine,
                -1,
                CycleMode.Yoyo
            );
        }

        private void PlayButtonClick()
        {
            _mainMenuMediator.PlayButtonClick();
        }
    }
}