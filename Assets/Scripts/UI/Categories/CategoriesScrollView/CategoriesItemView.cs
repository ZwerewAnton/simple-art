using UI.Common.ScrollView;
using UI.Pictures;
using UnityEngine;

namespace UI.Categories.CategoriesScrollView
{
    public class CategoriesItemView : ScrollItemView<CategoriesItemModel>
    {
        [SerializeField] private PicturesScrollPresenter scrollPresenter;

        public void Initialize()
        {
            scrollPresenter.Initialize();
        }

        public override void SetData(int itemIndex, CategoriesItemModel model)
        {
            base.SetData(itemIndex, model);
            
            scrollPresenter.UpdateModels(model.pictureItemModels);
        }
    }
}