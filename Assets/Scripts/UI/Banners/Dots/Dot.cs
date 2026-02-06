using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Banners.Dots
{
    public class Dot : MonoBehaviour
    {
        private const float OnAlpha = 1f;
        private const float OffAlpha = 0f;
        [SerializeField] private Image onImage;
        [SerializeField] private Image offImage;
        [SerializeField] private float fadeDuration = 0.25f;
        private Tween _fadeTween;

        private bool _isActive;

        public void ChangeActive(bool isActive)
        {
            if (_isActive == isActive)
                return;

            _isActive = isActive;

            _fadeTween.Stop();

            var targetAlpha = _isActive ? OnAlpha : OffAlpha;

            _fadeTween = Tween.Alpha(
                onImage,
                targetAlpha,
                fadeDuration,
                Ease.OutQuad
            );
        }
    }
}