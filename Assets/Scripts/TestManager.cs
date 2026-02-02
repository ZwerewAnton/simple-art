using System;
using System.Collections.Generic;
using UI.ScrollViews.CategoryScrollView;
using UnityEngine;
using UnityEngine.Serialization;

public class TestManager : MonoBehaviour
{
    [SerializeField] private CategoryScrollPresenter categoryScrollPresenter;

    private void Start()
    {
        var categories = new List<CategoryItemModel>
        {
            new CategoryItemModel { categoryName = "All", isLast = false },
            new CategoryItemModel { categoryName = "Odd", isLast = false },
            new CategoryItemModel { categoryName = "Even", isLast = true }
        };

        categoryScrollPresenter.Initialize(categories);
    }
}
