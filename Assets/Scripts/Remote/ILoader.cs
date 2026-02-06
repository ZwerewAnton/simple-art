using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Remote
{
    public interface ILoader
    {
        void Load(string url, Action<Texture2D, bool> onComplete);
    }
}