using System;
using Device;
using UnityEngine;
using Zenject;

namespace UI.Utils
{
    public class Resizer : MonoBehaviour
    {
        [SerializeField] private RectTransform bannerPanel;
        [SerializeField] private RectTransform viewsPanel;
        
        [Header("Values")]
        [SerializeField] private float phoneHeight = 721f;
        [SerializeField] private float tabletHeight = 504.7f;
        
        private IDeviceService _deviceService;

        [Inject]
        private void Construct(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        private void Awake()
        {
            SetPanelsSize();
        }

        private void SetPanelsSize()
        {
            if (_deviceService.Device == Device.Device.Phone)
                return;
            
            bannerPanel.sizeDelta = new Vector2(bannerPanel.sizeDelta.x, tabletHeight);
            viewsPanel.offsetMax = new Vector2(viewsPanel.offsetMax.x, -tabletHeight);
        }
    }
}