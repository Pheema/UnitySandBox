using System;
using System.Collections;
using UnityEngine;

// 以下のサイトのクラスを利用
// http://baba-s.hatenablog.com/entry/2016/07/05/100000
// http://baba-s.hatenablog.com/entry/2016/07/06/100000
namespace Slack
{
    [Serializable]
    public class PostMessageData
    {
        public string token = string.Empty;
        public string channel = string.Empty;
        public string text = string.Empty;
        public string parse = string.Empty;
        public string link_names = string.Empty;
        public string username = string.Empty;
        public string icon_url = string.Empty;
        public string icon_emoji = string.Empty;
    }

    [Serializable]
    public class UploadData
    {
        public string token = string.Empty;
        public string filename = string.Empty;
        public string title = string.Empty;
        public string initial_comment = string.Empty;
        public string channel = string.Empty;
    }

    public static class SlackAPI
    {
        public static IEnumerator PostMessage(
            PostMessageData data,
            Action onSuccess = null,
            Action<string> onError = null
        )
        {
            var form = new WWWForm();
            form.AddField("token", data.token);
            form.AddField("channel", data.channel);
            form.AddField("text", data.text);
            form.AddField("parse", data.parse);
            form.AddField("link_names", data.link_names);
            form.AddField("username", data.username);
            form.AddField("icon_url", data.icon_url);
            form.AddField("icon_emoji", data.icon_emoji);

            var url = "https://slack.com/api/chat.postMessage";
            var www = new WWW(url, form);
            yield return www;
            var error = www.error;

            if (!string.IsNullOrEmpty(error))
            {
                if (onError != null)
                {
                    onError(error);
                }
                yield break;
            }

            if (onSuccess != null)
            {
                onSuccess();
            }
        }

        public static IEnumerator UploadScreenShot(
            UploadData data,
            Action onSuccess = null,
            Action<string> onError = null
        )
        {
            yield return new WaitForEndOfFrame();

            var width = Screen.width;
            var height = Screen.height;
            var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            var source = new Rect(0, 0, width, height);

            texture.ReadPixels(source, 0, 0);
            texture.Apply();

            var form = new WWWForm();
            var contents = texture.EncodeToPNG();

            form.AddField("token", data.token);
            form.AddField("title", data.title);
            form.AddField("initial_comment", data.initial_comment);
            form.AddField("channels", data.channel);

            form.AddBinaryData("file", contents, data.filename, "image/png");

            var url = "https://slack.com/api/files.upload";
            var www = new WWW(url, form);
            yield return www;
            var error = www.error;

            if (!string.IsNullOrEmpty(error))
            {
                if (onError != null)
                {
                    onError(error);
                }
                yield break;
            }

            if (onSuccess != null)
            {
                onSuccess();
            }
        }

        public static IEnumerator UploadRenderTexture(
            RenderTexture rt,
            UploadData data,
            Action onSuccess = null,
            Action<string> onError = null
        )
        {
            var width = rt.width;
            var height = rt.height;
            var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            var source = new Rect(0, 0, width, height);

            RenderTexture.active = rt;
            texture.ReadPixels(source, 0, 0);
            texture.Apply();
            RenderTexture.active = null;

            var form = new WWWForm();
            var contents = texture.EncodeToPNG();

            form.AddField("token", data.token);
            form.AddField("title", data.title);
            form.AddField("initial_comment", data.initial_comment);
            form.AddField("channels", data.channel);

            form.AddBinaryData("file", contents, data.filename, "image/png");

            var url = "https://slack.com/api/files.upload";
            var www = new WWW(url, form);
            yield return www;
            var error = www.error;

            if (!string.IsNullOrEmpty(error))
            {
                if (onError != null)
                {
                    onError(error);
                }
                yield break;
            }

            if (onSuccess != null)
            {
                onSuccess();
            }
        }
    }
}