using Data;
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
        }

        private void BindSystemConfig()
        {
            Container.Bind<SystemConfig>().FromInstance(systemConfig).AsSingle().NonLazy();
        }
    }
}