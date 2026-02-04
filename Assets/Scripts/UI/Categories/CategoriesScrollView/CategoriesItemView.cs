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

        public Vector2 GetContentPosition()
        {
            return scrollPresenter.GetContentPosition();
        }

        public override void SetData(int itemIndex, CategoriesItemModel model)
        {
            base.SetData(itemIndex, model);
            
            scrollPresenter.UpdateModels(model.pictureItemModels);
            scrollPresenter.SetContentPosition(model.contentPosition);
        }
    }
}