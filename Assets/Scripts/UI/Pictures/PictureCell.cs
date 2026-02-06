using PrimeTween;
using Remote;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Pictures
{
    public class PictureCell :  MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image loading;
        [SerializeField] private Image badge;
        [SerializeField] private RawImage image;

        private ILoader _imageLoader;
        private string _currentUrl;
        private int _requestId;
        
        private Tween _loadingTween;

        [Inject]
        private void Construct(ILoader imageLoader)
        {
            _imageLoader = imageLoader;
        }
        
        public void SetSize(float size)
        {
            rectTransform.sizeDelta = new Vector2(size, size);
        }

        public void SetPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void SetBadge(bool active)
        {
            badge.enabled = active;
        }
        
        public void SetImage(string url)
        {
            _currentUrl = url;
            _requestId++;
            var localRequestId = _requestId;

            image.texture = null;

            _imageLoader.Load(url, (texture, fromCache) =>
            {
                if (localRequestId != _requestId || _currentUrl != url && !this)
                    return;

                if (gameObject != null  && !gameObject.activeInHierarchy)
                    return;

                if (!fromCache)
                {
                    ShowLoading();
                }

                image.texture = texture;

                if (!fromCache)
                {
                    HideLoadingSmooth();
                }
                else
                {
                    _loadingTween.Stop();
                    loading.gameObject.SetActive(false);
                }
            });
        }

        private void ShowLoading()
        {
            _loadingTween.Stop();
            loading.gameObject.SetActive(true);

            _loadingTween = Tween.Alpha(
                loading,
                0f,
                1f,
                duration: 0.2f,
                ease: Ease.OutQuad
            );
        }

        private void HideLoadingSmooth()
        {
            _loadingTween.Stop();

            _loadingTween = Tween.Alpha(
                loading,
                1f,
                0f,
                duration: 0.25f,
                ease: Ease.OutQuad
            ).OnComplete(() =>
            {
                loading.gameObject.SetActive(false);
            });
        }

        private void StopLoadingInstant()
        {
            _loadingTween.Stop();
            loading.gameObject.SetActive(false);
            loading.color = new Color(1, 1, 1, 1);
        }

        private void OnDisable()
        {
            _loadingTween.Stop();
        }
    }
}