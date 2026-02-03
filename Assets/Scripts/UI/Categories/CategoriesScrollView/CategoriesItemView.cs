using UI.Common.ScrollView;
using UI.Pictures;
using UnityEngine;

namespace UI.Categories.CategoriesScrollView
{
    public class CategoriesItemView : ScrollItemView<CategoriesItemModel>
    {
        [SerializeField] private PicturesScrollPresenter scrollPresenter;

        public void Initialize(CategoriesItemModel model)
        {
            scrollPresenter.Initialize(model.pictureItemModels);
        }
    }
}