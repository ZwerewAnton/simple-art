using System.Collections.Generic;
using UI.Common.ScrollView;
using UnityEngine;

namespace UI.Pictures
{
    public class PicturesScrollPresenter : ScrollPresenterBase<PictureItemModel[], PictureGridItemView>
    {
        [Header("Grid")] 
        [Range(2, 3)] [SerializeField] protected int itemsPerRow = 2;
        [SerializeField] protected float pictureCellSize = 640f;
        [SerializeField] protected float cellSpacing = 40f;
        [SerializeField] protected float horizontalPadding = 60f;

        public void Initialize(List<PictureItemModel> models)
        {
            base.Initialize(BuildRows(models));
        }
        
        protected override int CalculateViewItemCount(float cellSize)
        {
            return Mathf.CeilToInt(GetViewportSize() / cellSize) + additionalPoolItemsCount;
        }
        
        protected override void CreatePool()
        {
            base.CreatePool();

            foreach (var gridItemView in ActiveItems)
            {
                gridItemView.Initialize(itemsPerRow,  pictureCellSize, cellSpacing, horizontalPadding);
            }
        }
        
        private List<PictureItemModel[]> BuildRows(List<PictureItemModel> models)
        {
            var rows = new List<PictureItemModel[]>();

            for (var i = 0; i < models.Count; i += itemsPerRow)
            {
                var count = Mathf.Min(itemsPerRow, models.Count - i);
                var row = new PictureItemModel[count];

                for (var j = 0; j < count; j++)
                    row[j] = models[i + j];

                rows.Add(row);
            }

            return rows;
        }
    }
}