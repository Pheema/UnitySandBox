using UnityEngine;
using System.Collections;

public class JFA : MonoBehaviour {
    public int width = 128;
    public int height = 128;

    [SerializeField]
    Shader m_shader;
    Material m_material;

    Texture2D m_demoTex;
    RenderTexture m_voronoiRT;
    RenderTexture m_viewRT;

    public Texture2D testTex;

	void Start()
    {
        if (testTex)
        {
            width = testTex.width; height = testTex.height;
        }

        m_material = new Material(m_shader);
        
        m_voronoiRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        m_voronoiRT.filterMode = FilterMode.Point;
        m_viewRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        m_viewRT.filterMode = FilterMode.Point;

        m_demoTex = CreateDemoTex(20);
        Graphics.Blit(m_demoTex, m_voronoiRT, m_material, 0);
        StartCoroutine(CreateVoronoiCoroutine());
        //CreateVoronoi();
    }

    IEnumerator CreateVoronoiCoroutine()
    {
        float m_jumpSize = 0.5f;
        float invMaxLength = 1.0f / Mathf.Max(width, height);

        for (;;)
        {
            m_material.SetFloat("_JumpSize", m_jumpSize);
            var tempRT = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBFloat);
            tempRT.filterMode = FilterMode.Point;
            Graphics.Blit(m_voronoiRT, tempRT);
            m_material.SetTexture("_VoronoiTex", tempRT);
            Graphics.Blit((RenderTexture)null, m_voronoiRT, m_material, 1);
            RenderTexture.ReleaseTemporary(tempRT);
            m_jumpSize *= 0.5f;
            Graphics.Blit(m_voronoiRT, m_viewRT, m_material, 2);
            yield return new WaitForSeconds(1);
            if (m_jumpSize < invMaxLength) yield break;
        }
    }

    void CreateVoronoi()
    {
        float m_jumpSize = 0.5f;
        float invMaxLength = 1.0f / Mathf.Max(width, height);

        for (;;)
        {
            m_material.SetFloat("_JumpSize", m_jumpSize);
            var tempRT = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBFloat);
            tempRT.filterMode = FilterMode.Point;
            Graphics.Blit(m_voronoiRT, tempRT);
            m_material.SetTexture("_VoronoiTex", tempRT);
            Graphics.Blit((RenderTexture)null, m_voronoiRT, m_material, 1);
            RenderTexture.ReleaseTemporary(tempRT);
            m_jumpSize *= 0.5f;
            Graphics.Blit(m_voronoiRT, m_viewRT, m_material, 2);
            if (m_jumpSize < invMaxLength) return;
        }
    }

    void OnGUI()
    {
        int showWidth = width;
        float aspect = (float)width / height;
        int showHeight = (int)(showWidth / aspect);
        GUI.DrawTexture(new Rect(0, 0, showWidth, showHeight), m_demoTex);
        GUI.DrawTexture(new Rect(showWidth, 0, showWidth, showHeight), m_voronoiRT);
        GUI.DrawTexture(new Rect(2 * showWidth, 0, showWidth, showHeight), m_viewRT);
    }

    Texture2D CreateDemoTex(int pointNum)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        tex.filterMode = FilterMode.Point;
        // Fill the texture
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                tex.SetPixel(i, j, Color.black);
            }
        }

        // Set random points
        for (int i = 0; i < pointNum; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            tex.SetPixel(x, y, Color.white);
        }
        tex.Apply();
        return tex;
    }
}
