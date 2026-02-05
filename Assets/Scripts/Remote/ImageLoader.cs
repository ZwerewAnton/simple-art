using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Remote
{
    public class ImageLoader : IDisposable, ILoader
    {
        private readonly Dictionary<string, Texture2D> _cache = new();
        private readonly Dictionary<string, List<Action<Texture2D>>> _pendingCallbacks = new();
        private ILoader _loaderImplementation;

        public void Load(string url, Action<Texture2D> onComplete)
        {
            if (string.IsNullOrEmpty(url))
            {
                onComplete?.Invoke(null);
                return;
            }

            if (_cache.TryGetValue(url, out var cached))
            {
                onComplete?.Invoke(cached);
                return;
            }

            if (_pendingCallbacks.TryGetValue(url, out var callbacks))
            {
                callbacks.Add(onComplete);
                return;
            }

            _pendingCallbacks[url] = new List<Action<Texture2D>> { onComplete };
            LoadInternal(url).Forget();
        }

        private async UniTaskVoid LoadInternal(string url)
        {
            using var request = UnityWebRequestTexture.GetTexture(url, false);
            request.timeout = 10;
            
            await request.SendWebRequest().ToUniTask();

            Texture2D tex = null;
            if (request.result == UnityWebRequest.Result.Success)
            {
                tex = DownloadHandlerTexture.GetContent(request);
                _cache[url] = tex;
            }
            else
            {
                Debug.LogWarning($"Image load failed: {url} with {request.error}");
            }

            if (_pendingCallbacks.TryGetValue(url, out var callbacks))
            {
                foreach (var callback in callbacks)
                    callback?.Invoke(tex);

                _pendingCallbacks.Remove(url);
            }
        }

        private void ClearCache()
        {
            foreach (var tex in _cache.Values)
                Object.Destroy(tex);
            _cache.Clear();
        }

        public void Dispose()
        {
            ClearCache();
        }
    }
}
