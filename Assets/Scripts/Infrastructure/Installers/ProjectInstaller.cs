using Data;
using Device;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private SystemConfig  systemConfig;

        public override void InstallBindings()
        {
            BindSystemConfig();
            BindDeviceService();
        }

        private void BindDeviceService()
        {
            Container.Bind<IDeviceService>().To<DeviceService>().AsSingle();
        }

        private void BindSystemConfig()
        {
            Container.Bind<SystemConfig>().FromInstance(systemConfig).AsSingle().NonLazy();
        }
    }
}