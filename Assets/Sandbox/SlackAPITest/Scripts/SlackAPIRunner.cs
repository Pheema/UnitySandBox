using UnityEngine;
using Slack;
using System.Collections;

public class SlackAPIRunner : MonoBehaviour
{
    public string token;
    public string channel;

    RenderTexture m_rtLarge;
    RenderTexture m_rtSmall;

    void Start()
    {
        m_rtLarge = new RenderTexture(Screen.width, Screen.height, 24);
        m_rtSmall = new RenderTexture(Screen.width / 2, Screen.height / 2, 24);
        StartCoroutine(UploadRT());
    }

    IEnumerator UploadRT()
    {
        yield return new WaitForEndOfFrame();
        var uploadData = new UploadData
        {
            token = token,
            channel = channel,
            title = "RenderTextureのアップロードテスト",
            filename = "screenShot.png",
            initial_comment = "hoge"
        };

        Graphics.Blit(null, m_rtLarge);
        Graphics.Blit(m_rtLarge, m_rtSmall);
        var routine = SlackAPI.UploadRenderTexture(m_rtSmall, uploadData);
        StartCoroutine(routine);
        Debug.Log("Uploaded");
    }
}
