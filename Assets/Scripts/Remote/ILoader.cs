using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Remote
{
    public interface ILoader
    {
        public void Load(string url, Action<Texture2D> onComplete);
        //public UniTask<Texture2D> Load(string url);
    }
}