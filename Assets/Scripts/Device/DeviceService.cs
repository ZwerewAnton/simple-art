using UnityEngine.Device;

namespace Device
{
    public class DeviceService : IDeviceService
    {
        private DeviceService()
        {
            Device = GetDeviceType();
        }

        public Device Device { get; }

        private static Device GetDeviceType()
        {
            return Screen.width / (1f * Screen.height) >= 0.65f ? Device.Tablet : Device.Phone;
        }
    }
}