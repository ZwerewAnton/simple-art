using PrimeTween;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace UI.Banners
{
    public class CozyBanner : BannerView
    {
        [Header("Elements")]
        [SerializeField] private RectTransform[] stars;
        [SerializeField] private RectTransform rays;
        [SerializeField] private RectTransform button;
        [SerializeField] private RectTransform logo;
        [SerializeField] private RectTransform brush;
        [SerializeField] private RectTransform mask;
        [SerializeField] private ParticleSystem particleSystem;
        
        [Header("Rays")]
        [SerializeField] private float raysPulseScale = 0.1f;
        [SerializeField] private float raysPulseDuration = 2f;
        [SerializeField] private float raysAngle = 50f;
        [SerializeField] private float raysRotationDuration = 12f;
        [Header("Brush")]
        [SerializeField] private float brushAngle = 2.5f;
        [SerializeField] private float brushDuration = 3f;
        [Header("Stars")]
        [SerializeField] private float starsAngle = 10f;
        [Header("Items")]
        [SerializeField] private float itemsAngle = 2.5f;
        [SerializeField] private float itemsDuration = 3f;
        [Header("Mask")]
        [SerializeField] private float maskDuration = 3f;
        [Header("Button")]
        [SerializeField] private float minScale = 0.6f;
        [SerializeField] private float pressDuration = 0.2f;
        [SerializeField] private float buttonAngle = 10f;
        [SerializeField] private float buttonRotationDuration = 3f;
        
        private Sequence _buttonPressSequence;
        private Sequence _paintSequence;
        private Sequence _starsSequence;
        private Tween _brushTween;
        private Sequence _logoRaySequence;
        private Tween _buttonRotateTween;
        private Tween _rayTween;

        protected override void StartAnimation()
        {
            StopAnimation();

            particleSystem.Play();
            AnimateButton();
            AnimateStars();
            AnimatePaint();
            AnimateRaysAndLogo();
        }

        protected override void StopAnimation()
        {
            if (_buttonPressSequence.isAlive)
                _buttonPressSequence.Stop();
            if (_rayTween.isAlive)
                _rayTween.Stop();
            if (_starsSequence.isAlive)
                _starsSequence.Stop();
            if (_logoRaySequence.isAlive)
                _logoRaySequence.Stop();
            if (_paintSequence.isAlive)
                _paintSequence.Stop();
            if (_brushTween.isAlive)
                _brushTween.Stop();
            if (_buttonRotateTween.isAlive)
                _buttonRotateTween.Stop();
            particleSystem.Stop();
        }

        private void AnimateButton()
        {
            _buttonPressSequence = Sequence.Create(cycles: -1);

            _buttonPressSequence.ChainDelay(2f);
            _buttonPressSequence.Chain(Tween.Scale(button, Vector3.one * minScale, pressDuration, Ease.OutBack));
            _buttonPressSequence.Chain(Tween.Scale(button, Vector3.one, pressDuration, Ease.OutBack));
            _buttonRotateTween = Tween.LocalRotation(
                button,
                new Vector3(0, 0, buttonAngle),
                new Vector3(0, 0, -buttonAngle),
                buttonRotationDuration,
                Ease.InOutSine,
                cycles: -1,
                cycleMode:CycleMode.Yoyo
            );
        }

        private void AnimateStars()
        {
            _starsSequence = Sequence.Create(cycles: -1, Sequence.SequenceCycleMode.Yoyo);
            
            for (var i = 0; i < stars.Length; i++)
            {
                var angle = UnityEngine.Random.Range(-starsAngle, starsAngle);

                _starsSequence.Group(
                    Tween.LocalRotation(
                        stars[i],
                        new Vector3(0, 0, -angle),
                        new Vector3(0, 0, angle),
                        2f,
                        Ease.InOutSine
                    )
                );
            }
        }
        
        private void AnimateRaysAndLogo()
        {
            _rayTween = Tween.LocalRotation(
                rays,
                new Vector3(0, 0, -raysAngle),
                new Vector3(0, 0, raysAngle),
                raysRotationDuration,
                Ease.InOutSine,
                cycles: -1,
                cycleMode:CycleMode.Yoyo
            );
            
            _logoRaySequence = Sequence.Create(cycles: -1, Sequence.SequenceCycleMode.Yoyo);
            
            _logoRaySequence.Group(
                Tween.Scale(
                    rays,
                    Vector3.one * (1f - raysPulseScale),
                    Vector3.one * (1f + raysPulseScale),
                    raysPulseDuration,
                    Ease.InOutSine
                )
            );
            
            _logoRaySequence.Group(
                Tween.Scale(
                    logo,
                    Vector3.one * (1f - raysPulseScale),
                    Vector3.one * (1f + raysPulseScale),
                    raysPulseDuration,
                    Ease.InOutSine
                )
            );
        }

        private void AnimatePaint()
        {
            mask.sizeDelta = new Vector2(1f, 1f);
            
            _paintSequence = Sequence.Create(cycles: -1, Sequence.SequenceCycleMode.Restart);
            
            _paintSequence.Group(Tween.LocalRotation(
                brush,
                Vector3.zero,
                new Vector3(0, 0, brushAngle),
                brushDuration,
                Ease.InOutSine,
                cycles: 2,
                cycleMode: CycleMode.Yoyo
            ));

            _paintSequence.Chain(Tween.UISizeDelta(
                    mask,
                    new Vector2(1, 1),
                    new Vector2(2000, 2000),
                    1f,
                    Ease.InOutSine
                )
            );
            
            _paintSequence.ChainDelay(3f);
        }
    }
}