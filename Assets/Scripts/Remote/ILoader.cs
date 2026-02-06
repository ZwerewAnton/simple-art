using System;
using UnityEngine;

namespace Remote
{
    public interface ILoader
    {
        void Load(string url, Action<Texture2D, bool> onComplete);
        bool HasImage(string url, out Texture2D image);
    }
}