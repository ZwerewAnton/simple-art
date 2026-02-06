using System;
using System.Collections.Generic;
using System.Text;
using Data;
using UI.Categories.CategoriesScrollView;
using UI.Categories.CategoryScrollView;
using UI.Mediators;
using UI.Pictures;
using UnityEngine;
using Zenject;

namespace MainMenu
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        private MainMenuMediator _mainMenuMediator;
        private SystemConfig _systemConfig;
        
        [Inject]
        private void Construct(MainMenuMediator mainMenuMediator, SystemConfig systemConfig)
        {
            _mainMenuMediator  = mainMenuMediator;
            _systemConfig = systemConfig;
        }

        private void Awake()
        {
            Application.targetFrameRate = 120;
        }

        private void Start()
        {
            InitializeScrolls();
        }

        private void InitializeScrolls()
        {
            var url = _systemConfig.URL;
            
            var tabs = new List<CategoryItemModel>
            {
                new CategoryItemModel { categoryName = "All", isLast = false, isSelected = true},
                new CategoryItemModel { categoryName = "Odd", isLast = false },
                new CategoryItemModel { categoryName = "Even", isLast = true }
            };
            _mainMenuMediator.InitializeTabs(tabs);

            var picturesOdd = new List<PictureItemModel>();
            var picturesEven = new List<PictureItemModel>();
            var picturesAll = new List<PictureItemModel>();

            for (var i = 1; i <= 66; i++)
            {
                var pictureModel = new PictureItemModel
                {
                    ImageUrl = url + "/" + i + ".jpg",
                    IsPremium = i % 4 == 0
                };
                
                picturesAll.Add(pictureModel);
                
                if (i % 2 == 0)
                    picturesEven.Add(pictureModel);
                else
                    picturesOdd.Add(pictureModel);
            }
            var categoriesList = new List<CategoriesItemModel>()
            {
                new CategoriesItemModel { pictureItemModels = picturesAll },
                new CategoriesItemModel { pictureItemModels = picturesOdd },
                new CategoriesItemModel { pictureItemModels = picturesEven }
            };
            _mainMenuMediator.InitializeCategories(categoriesList);
        }
    }
}