using Cysharp.Threading.Tasks;
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
        [SerializeField] private RawImage image;

        private ILoader _imageLoader;
        private string _currentUrl;
        private int _requestId;

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
        
        public void SetImage(string url)
        {
            _currentUrl = url;
            _requestId++;

            image.texture = null;
            loading.gameObject.SetActive(true);

            var localRequestId = _requestId;
            _imageLoader.Load(url, texture =>
            {
                // проверка, актуальна ли картинка
                if (localRequestId != _requestId || _currentUrl != url)
                    return;

                image.texture = texture;
                loading.gameObject.SetActive(false);
            });
        }
    }
}