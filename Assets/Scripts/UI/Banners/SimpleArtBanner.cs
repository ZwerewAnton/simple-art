using PrimeTween;
using UnityEngine;

namespace UI.Banners
{
    public class SimpleArtBanner : BannerView
    {
        [Header("Elements")] [SerializeField] private RectTransform[] lights;

        [SerializeField] private RectTransform rays;
        [SerializeField] private RectTransform movingFrame;
        [SerializeField] private RectTransform simpleArtLogo;

        [Header("Lights")] [SerializeField] private float lightScaleDelta = 0.08f;

        [SerializeField] private float lightDuration = 1.2f;

        [Header("Rays and Logo")] [SerializeField]
        private float raysPulseScale = 0.1f;

        [SerializeField] private float raysPulseDuration = 2f;
        [SerializeField] private float raysAngle = 50f;
        [SerializeField] private float raysRotationDuration = 12f;

        [Header("Frame")] [SerializeField] private float frameAngle = 2.5f;

        [SerializeField] private float frameDuration = 3f;
        private Tween _frameTween;

        private Sequence _lightSequence;
        private Sequence _logoRaySequence;
        private Tween _rayTween;

        protected override void StartAnimation()
        {
            StopAnimation();

            AnimateLights();
            AnimateRaysAndLogo();
            AnimateFrame();
        }

        protected override void StopAnimation()
        {
            if (_lightSequence.isAlive)
                _lightSequence.Stop();
            if (_logoRaySequence.isAlive)
                _logoRaySequence.Stop();
            if (_frameTween.isAlive)
                _frameTween.Stop();
            if (_rayTween.isAlive)
                _rayTween.Stop();
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

        private void AnimateRaysAndLogo()
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

            _logoRaySequence = Sequence.Create(-1, Sequence.SequenceCycleMode.Yoyo);

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
                    simpleArtLogo,
                    Vector3.one * (1f - raysPulseScale),
                    Vector3.one * (1f + raysPulseScale),
                    raysPulseDuration,
                    Ease.InOutSine
                )
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
    }
}