using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace Device
{
    public class DeviceService : IDeviceService
    {
        private const float TabletThreshold = 0.59f;
        
        private DeviceService()
        {
            Device = GetDeviceType();
        }

        public Device Device { get; }

        private static Device GetDeviceType()
        {
            float width  = Screen.width;
            float height = Screen.height;
            
            var aspect = Mathf.Min(width, height) / Mathf.Max(width, height);
            
            return aspect >= TabletThreshold
                ? Device.Tablet
                : Device.Phone;
        }
    }
}