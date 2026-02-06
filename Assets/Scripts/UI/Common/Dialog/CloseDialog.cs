using UnityEngine;
using UnityEngine.UI;

namespace UI.Common.Dialog
{
    public class CloseDialog : BaseDialog
    {
        [SerializeField] private Button closeButton;

        protected virtual void OnEnable()
        {
            closeButton.onClick.AddListener(Close);
        }

        protected virtual void OnDisable()
        {
            closeButton.onClick.RemoveListener(Close);
        }
    }
}