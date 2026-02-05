using System;
using System.Collections;
using System.Collections.Generic;
using UI.Categories.CategoriesScrollView;
using UI.Categories.CategoryScrollView;
using UI.Pictures;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    [SerializeField] private CategoryScrollPresenter categoryScrollPresenter;
    [SerializeField] private CategoriesScrollPresenter categoriesScrollPresenter;

    private void Awake()
    {
        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        var categories = new List<CategoryItemModel>
        {
            new CategoryItemModel { categoryName = "All", isLast = false, isSelected = true},
            new CategoryItemModel { categoryName = "Odd", isLast = false },
            new CategoryItemModel { categoryName = "Even", isLast = true }
        };
        categoryScrollPresenter.Initialize(categories);

        var pictures = new List<PictureItemModel>
        {
            new PictureItemModel { ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/1.jpg" },
            new PictureItemModel { ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/2.jpg" },
            new PictureItemModel { ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/3.jpg" },
            new PictureItemModel { ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/4.jpg" },
            new PictureItemModel { ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/5.jpg" },
            new PictureItemModel { ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/6.jpg" },
            new PictureItemModel { ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/7.jpg" },
            new PictureItemModel { ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/8.jpg" },
            new PictureItemModel { ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/9.jpg" },
            new PictureItemModel { ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/10.jpg" },
            new PictureItemModel { ImageUrl = "http://data.ikppbb.com/test-task-unity-data/pics/11.jpg" },
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
