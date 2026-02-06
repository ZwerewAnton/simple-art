using UnityEngine;

namespace UI.Banners
{
    public abstract class BannerView : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            StartAnimation();
        }

        protected virtual void OnDisable()
        {
            StopAnimation();
        }

        protected abstract void StopAnimation();
        protected abstract void StartAnimation();
    }
}