using UI.Categories.CategoriesScrollView;
using UI.Categories.CategoryScrollView;
using UnityEngine;

namespace UI.Categories
{
    public class CategoriesScrollCoordinator : MonoBehaviour
    {
        [SerializeField] private CategoriesScrollPresenter categoriesScrollPresenter;
        [SerializeField] private CategoryScrollPresenter categoryScrollPresenter;
        [SerializeField] private SelectionIndicator selectionIndicator;

        private void OnEnable()
        {
            categoryScrollPresenter.Initialized += OnCategoryScrollInitialized;
            categoriesScrollPresenter.CenteredViewChanged += OnCategoriesScrollCenteredItemChanged;
            categoryScrollPresenter.ItemClicked += OnCategoryItemClicked;
        }

        private void OnDisable()
        {
            categoriesScrollPresenter.CenteredViewChanged -= OnCategoriesScrollCenteredItemChanged;
            categoryScrollPresenter.ItemClicked -= OnCategoryItemClicked;
        }

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
    }
}