using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using System.Threading;

namespace Remote
{
    public class ImageLoader : IDisposable, ILoader
    {
        private readonly Dictionary<string, Texture2D> _cache = new();
        private readonly Dictionary<string, List<Action<Texture2D>>> _pendingCallbacks = new();

        private readonly CancellationTokenSource _cts = new();

        public void Load(string url, Action<Texture2D, bool> onComplete)
        {
            if (string.IsNullOrEmpty(url))
            {
                onComplete?.Invoke(null, true);
                return;
            }

            if (_cache.TryGetValue(url, out var cached))
            {
                onComplete?.Invoke(cached, true);
                return;
            }

            if (_pendingCallbacks.TryGetValue(url, out var callbacks))
            {
                callbacks.Add(tex => onComplete?.Invoke(tex, false));
                return;
            }

            _pendingCallbacks[url] = new List<Action<Texture2D>>
            {
                tex => onComplete?.Invoke(tex, false)
            };

            LoadInternal(url, _cts.Token).Forget();
        }

        public bool HasImage(string url, out Texture2D image)
        {
            return _cache.TryGetValue(url, out image);
        }

        private async UniTaskVoid LoadInternal(string url, CancellationToken token)
        {
            using var request = UnityWebRequestTexture.GetTexture(url, false);
            request.timeout = 10;

            try
            {
                await request
                    .SendWebRequest()
                    .ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                _pendingCallbacks.Remove(url);
                return;
            }

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
            _cts.Cancel();
            _cts.Dispose();

            ClearCache();
            _pendingCallbacks.Clear();
        }
    }
}
