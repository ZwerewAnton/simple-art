using UnityEngine;
using UnityEngine.UI;

namespace UI.Common.Dialog
{
    public class CloseDialog : BaseDialog
    {
        [SerializeField] private Button closeButton;

        private void OnEnable()
        {
            closeButton.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(Close);
        }
    }
}