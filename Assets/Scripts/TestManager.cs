using System;
using System.Collections.Generic;
using UI.Categories.CategoriesScrollView;
using UI.Categories.CategoryScrollView;
using UI.Pictures;
using UI.ScrollViews.CategoryScrollView;
using UnityEngine;
using UnityEngine.Serialization;

public class TestManager : MonoBehaviour
{
    [SerializeField] private CategoryScrollPresenter categoryScrollPresenter;
    [SerializeField] private CategoriesScrollPresenter categoriesScrollPresenter;

    private void Start()
    {
        var categories = new List<CategoryItemModel>
        {
            new CategoryItemModel { categoryName = "All", isLast = false },
            new CategoryItemModel { categoryName = "Odd", isLast = false },
            new CategoryItemModel { categoryName = "Even", isLast = true }
        };
        categoryScrollPresenter.Initialize(categories);

        var pictures = new List<PictureItemModel>
        {
            new PictureItemModel { },
            new PictureItemModel { },
            new PictureItemModel { },
            new PictureItemModel { },
            new PictureItemModel { },
            new PictureItemModel { },
            new PictureItemModel { },
            new PictureItemModel { },
            new PictureItemModel { },
            new PictureItemModel { },
            new PictureItemModel { },
        };
        var categoriesList = new List<CategoriesItemModel>()
        {
            new CategoriesItemModel { pictureItemModels = pictures },
            new CategoriesItemModel { pictureItemModels = pictures },
            new CategoriesItemModel { pictureItemModels = pictures },
        };
        categoriesScrollPresenter.Initialize(categoriesList);
    }
}
