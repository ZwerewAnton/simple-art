using Remote;
using SFX;
using UI.Mediators;
using UI.ScrollViews;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField] private NestedScrollCoordinator nestedScrollCoordinator;
        [SerializeField] private MainMenuMediator mainMenuMediator;
        [SerializeField] private SfxPlayer sfxPlayer;
        
        public override void InstallBindings()
        {
            BindSfxPlayer();
            BindNestedScrollCoordinator();
            BindImageLoader();
            BindMainMenuMediator();
        }

        private void BindSfxPlayer()
        {
            Container.Bind<SfxPlayer>().FromInstance(sfxPlayer).AsSingle().NonLazy();
        }

        private void BindMainMenuMediator()
        {
            Container.Bind<MainMenuMediator>().FromInstance(mainMenuMediator).AsSingle().NonLazy();
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