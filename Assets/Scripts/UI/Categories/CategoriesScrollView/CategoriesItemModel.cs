using System.Collections.Generic;
using UI.Pictures;
using Vector2 = UnityEngine.Vector2;

namespace UI.Categories.CategoriesScrollView
{
    public class CategoriesItemModel
    {
        public Vector2 contentPosition;
        public List<PictureItemModel> pictureItemModels;
    }
}