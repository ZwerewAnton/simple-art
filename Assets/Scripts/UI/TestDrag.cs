using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class TestDrag : MonoBehaviour, IBeginDragHandler, IDragHandler,
        IEndDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBeginDrag");
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("OnDrag");
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag");
        }
    }
}