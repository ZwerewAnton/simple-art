using System.Collections.Generic;
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

        private void Awake()
        {
            Application.targetFrameRate = 120;
        }

        private void Start()
        {
            InitializeScrolls();
        }

        [Inject]
        private void Construct(MainMenuMediator mainMenuMediator, SystemConfig systemConfig)
        {
            _mainMenuMediator = mainMenuMediator;
            _systemConfig = systemConfig;
        }

        private void InitializeScrolls()
        {
            var url = _systemConfig.URL;

            var tabs = new List<CategoryItemModel>
            {
                new() { categoryName = "All", isLast = false, isSelected = true },
                new() { categoryName = "Odd", isLast = false },
                new() { categoryName = "Even", isLast = true }
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

            var categoriesList = new List<CategoriesItemModel>
            {
                new() { pictureItemModels = picturesAll },
                new() { pictureItemModels = picturesOdd },
                new() { pictureItemModels = picturesEven }
            };
            _mainMenuMediator.InitializeCategories(categoriesList);
        }
    }
}