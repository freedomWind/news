using System;
using UnityEngine.Networking;

namespace NewsFramework.Services.Content.Http
{
    public sealed class UnityWebRequestContentHttpClient : IContentHttpClient
    {
        public void Send(ContentHttpRequest request, Action<ContentHttpResponse> onComplete)
        {
            if (request == null || string.IsNullOrEmpty(request.url))
            {
                onComplete?.Invoke(new ContentHttpResponse
                {
                    success = false,
                    errorKind = ContentHttpErrorKind.InvalidRequest,
                    error = "HTTP request URL is empty."
                });
                return;
            }

            UnityWebRequest unityRequest;
            try
            {
                unityRequest = CreateUnityRequest(request);
            }
            catch (Exception exception)
            {
                onComplete?.Invoke(new ContentHttpResponse
                {
                    success = false,
                    errorKind = ContentHttpErrorKind.InvalidRequest,
                    error = exception.Message
                });
                return;
            }

            var operation = unityRequest.SendWebRequest();
            operation.completed += _ =>
            {
                var response = new ContentHttpResponse
                {
                    statusCode = unityRequest.responseCode,
                    body = unityRequest.downloadHandler != null ? unityRequest.downloadHandler.text : string.Empty,
                    error = unityRequest.error
                };

                response.success = unityRequest.result == UnityWebRequest.Result.Success
                    && response.statusCode >= 200
                    && response.statusCode < 300;

                if (!response.success && string.IsNullOrEmpty(response.error))
                {
                    response.error = "HTTP request failed with status " + response.statusCode;
                }

                ContentHttpErrorClassifier.Normalize(response);
                unityRequest.Dispose();
                onComplete?.Invoke(response);
            };
        }

        private static UnityWebRequest CreateUnityRequest(ContentHttpRequest request)
        {
            var method = string.IsNullOrEmpty(request.method) ? ContentHttpMethods.Get : request.method;
            var unityRequest = new UnityWebRequest(request.url, method)
            {
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = request.timeoutSeconds > 0 ? request.timeoutSeconds : 10
            };

            if (method == ContentHttpMethods.Post)
            {
                var body = string.IsNullOrEmpty(request.body) ? string.Empty : request.body;
                unityRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(body));
                unityRequest.SetRequestHeader("Content-Type", "application/json");
            }

            if (request.headers != null)
            {
                for (var i = 0; i < request.headers.Count; i++)
                {
                    var header = request.headers[i];
                    if (header == null || string.IsNullOrEmpty(header.key))
                    {
                        continue;
                    }

                    unityRequest.SetRequestHeader(header.key, header.value ?? string.Empty);
                }
            }

            return unityRequest;
        }
    }
}
