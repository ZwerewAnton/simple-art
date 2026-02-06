using System.Collections.Generic;
using Remote;
using UI.Banners;
using UI.Banners.Dots;
using UI.Categories.CategoriesScrollView;
using UI.Categories.CategoryScrollView;
using UI.Premium;
using UnityEngine;
using Zenject;

namespace UI.Mediators
{
    public class MainMenuMediator : MonoBehaviour
    {
        [SerializeField] private CategoriesScrollPresenter categoriesScrollPresenter;
        [SerializeField] private CategoryScrollPresenter categoryScrollPresenter;
        [SerializeField] private SelectionIndicator selectionIndicator;
        [SerializeField] private BannerScrollPresenter bannerScrollPresenter;
        [SerializeField] private DotPresenter dotPresenter;
        [SerializeField] private PremiumDialog premiumDialog;
        [SerializeField] private ImageDialog.ImageDialog imageDialog;

        private ILoader _imageLoader;

        [Inject]
        private void Construct(ILoader imageLoader)
        {
            _imageLoader = imageLoader;
        }

        private void OnEnable()
        {
            categoryScrollPresenter.Initialized += OnCategoryScrollInitialized;
            categoriesScrollPresenter.CenteredViewChanged += OnCategoriesScrollCenteredItemChanged;
            categoryScrollPresenter.ItemClicked += OnCategoryItemClicked;
            bannerScrollPresenter.Initialized += OnBannerScrollInitialized;
            bannerScrollPresenter.FocusItemChanged += OnBannerFocusItemChanged;
        }

        private void OnDisable()
        {
            categoriesScrollPresenter.CenteredViewChanged -= OnCategoriesScrollCenteredItemChanged;
            categoryScrollPresenter.ItemClicked -= OnCategoryItemClicked;
            bannerScrollPresenter.Initialized -= OnBannerScrollInitialized;
            bannerScrollPresenter.FocusItemChanged -= OnBannerFocusItemChanged;
        }
        
        public void InitializeTabs(List<CategoryItemModel> categoryItemModels) 
            => categoryScrollPresenter.Initialize(categoryItemModels);
        
        public void InitializeCategories(List<CategoriesItemModel> categoriesItemModels) 
            => categoriesScrollPresenter.Initialize(categoriesItemModels);
        
        public void ShowImageDialog(string url)
        {
            var result = _imageLoader.HasImage(url, out var image);
            if (result)
                imageDialog.Show(image);
        }
        
        public void ShowPremiumDialog() => premiumDialog.Show();

        private void OnCategoriesScrollCenteredItemChanged(int index)
        {
            categoryScrollPresenter.SelectModel(index);
            selectionIndicator.MoveByIndex(index);
        }

        private void OnCategoryItemClicked(int index)
        {
            categoriesScrollPresenter.MoveToItem(index);
            selectionIndicator.MoveByIndex(index);
        }
        
        private void OnCategoryScrollInitialized() 
            => selectionIndicator.Setup(categoryScrollPresenter.ItemSize, categoryScrollPresenter.ModelsCount);

        private void OnBannerScrollInitialized() 
            => dotPresenter.SpawnDots(bannerScrollPresenter.ItemsCount, bannerScrollPresenter.FocusItemIndex);
        
        private void OnBannerFocusItemChanged(int index) 
            => dotPresenter.SetDotActive(index);
    }
}