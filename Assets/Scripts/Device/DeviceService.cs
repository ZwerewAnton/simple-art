using UnityEngine.Device;

namespace Device
{
    public class DeviceService : IDeviceService
    {
        public Device Device { get; }

        private DeviceService()
        {
            Device = GetDeviceType();
        }

        private static Device GetDeviceType()
        {
            return Screen.width / (1f * Screen.height) >= 0.65f ? Device.Tablet : Device.Phone;
        }
    }
}