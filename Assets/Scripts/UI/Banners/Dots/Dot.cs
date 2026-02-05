using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Banners.Dots
{
    public class Dot :  MonoBehaviour
    {
        [SerializeField] private Image onImage;
        [SerializeField] private Image offImage;
        [SerializeField] private float fadeDuration = 0.25f;

        private bool _isActive;
        private Tween _fadeTween;
        
        private const float OnAlpha = 1f;
        private const float OffAlpha = 0f;
        
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