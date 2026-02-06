using UnityEngine;

namespace UI.Banners
{
    public abstract class BannerView : MonoBehaviour
    {
        private void OnEnable()
        {
            StartAnimation();
        }

        private void OnDisable()
        {
            StopAnimation();
        }

        protected abstract void StopAnimation();
        protected abstract void StartAnimation();
    }
}