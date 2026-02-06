using UnityEngine;

namespace Data
{
    [CreateAssetMenu(
        fileName = "SystemConfig",
        menuName = "Configs/System Config",
        order = 0
    )]
    public class SystemConfig : ScriptableObject
    {
        [SerializeField] private string url;

        public string URL => url;
    }
}