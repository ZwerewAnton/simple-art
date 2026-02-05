using Remote;
using UI.ScrollViews;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField] private NestedScrollCoordinator nestedScrollCoordinator;
        
        public override void InstallBindings()
        {
            BindNestedScrollCoordinator();
            BindImageLoader();
        }

        private void BindImageLoader()
        {
            Container.Bind<ILoader>().To<ImageLoader>().AsSingle();
        }

        private void BindNestedScrollCoordinator()
        {
            Container.Bind<NestedScrollCoordinator>().FromInstance(nestedScrollCoordinator).AsSingle().NonLazy();
        }
    }
}