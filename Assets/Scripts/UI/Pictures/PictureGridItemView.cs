using System.Collections.Generic;
using UI.Common.ScrollView;
using UnityEngine;
using Zenject;

namespace UI.Pictures
{
    public class PictureGridItemView : ScrollItemView<PictureItemModel[]>
    {
        [SerializeField] private PictureCell cellPrefab;

        private DiContainer _diContainer;
        
        private readonly List<PictureCell> _cells = new();

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        public void Initialize(
            int itemsPerRow,
            float cellSize,
            float cellSpacing,
            float horizontalPadding)
        {
            CreateCells(itemsPerRow, cellSize);
            LayoutCells(cellSize, cellSpacing, horizontalPadding);
        }
        
        public override void SetData(int itemIndex, PictureItemModel[] model)
        {
            base.SetData(itemIndex, model);

            var modelCount = model.Length;
            for (var i = 0; i < _cells.Count; i++)
            {
                var cell = _cells[i];
                if (i >= modelCount)
                {
                    cell.SetActive(false);
                    continue;
                }

                cell.SetActive(true);
                cell.SetImage(model[i].ImageUrl);
            }
        }

        private void CreateCells(int count, float cellSize)
        {
            while (_cells.Count < count)
            {
                var go = _diContainer.InstantiatePrefab(cellPrefab, RectTransform);
                var cell = go.GetComponent<PictureCell>();
                cell.SetSize(cellSize);
                _cells.Add(cell);
            }
        }
        
        private void LayoutCells(
            float cellSize,
            float cellSpacing,
            float horizontalPadding)
        {
            var count = _cells.Count;

            var totalWidth =
                count * cellSize +
                (count - 1) * cellSpacing +
                horizontalPadding * 2;

            var startX = -totalWidth * 0.5f
                         + horizontalPadding
                         + cellSize * 0.5f;

            for (var i = 0; i < count; i++)
            {
                var x = startX + i * (cellSize + cellSpacing);
                _cells[i].SetPosition(new Vector2(x, 0f));
            }
        }
    }
}