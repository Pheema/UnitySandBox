using UnityEngine;
using UnityEngine.Rendering;

public class StableFluidSolver : MonoBehaviour
{
    [SerializeField]
    Shader m_shader;

    [SerializeField]
    Shader m_tex2View;

    Material m_material;
    Material m_tex2ViewMat;

    public int width = 128;
    public int height = 128;

    RenderTexture m_pressureRT;
    RenderTexture m_velocityRT;
    RenderTexture m_imageRT;

    [SerializeField]
    Texture2D tex;
    RenderTexture viewRT;

    RenderTexture m_prevImage;

    void Start()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth | DepthTextureMode.MotionVectors;

        viewRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);

        m_material = new Material(m_shader);
        m_tex2ViewMat = new Material(m_tex2View);

        m_pressureRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        m_pressureRT.wrapMode = TextureWrapMode.Repeat;
        m_velocityRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        m_velocityRT.wrapMode = TextureWrapMode.Repeat;
        m_imageRT = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        m_velocityRT.wrapMode = TextureWrapMode.Repeat;

        m_prevImage = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat);
        m_prevImage.wrapMode = TextureWrapMode.Repeat;

        var cb = new CommandBuffer();
        cb.name = "Save Previous frame.";
        cb.Blit(BuiltinRenderTextureType.CameraTarget, m_prevImage);
        GetComponent<Camera>().AddCommandBuffer(CameraEvent.AfterEverything, cb);

        if (tex == null)
        {
            tex = new Texture2D(m_pressureRT.width, m_pressureRT.height);

            float size = 10.0f;
            for (int j = 0; j < m_velocityRT.height; j++)
            {
                for (int i = 0; i < m_velocityRT.width; i++)
                {
                    float valR = Mathf.PerlinNoise(size * (float)i / m_velocityRT.width + 1.2f, size * (float)j / m_velocityRT.height);
                    float valG = Mathf.PerlinNoise(size * (float)i / m_velocityRT.width + 5.7f, size * (float)j / m_velocityRT.height);
                    // tex.SetPixel(i, j, Color.black);
                    tex.SetPixel(i, j, new Color(valR, valG, 0.0f, 1.0f));
                }
            }
            tex.Apply();
        }

        Graphics.Blit(tex, m_imageRT);

        m_material.SetFloat("_Density", 1000.0f);   // water
    }

    void FixedUpdate()
    {
        m_material.SetFloat("_FixedDeltaTime", Time.fixedDeltaTime);
        var tempRT = RenderTexture.GetTemporary(m_velocityRT.width, m_velocityRT.height, 0, RenderTextureFormat.ARGBFloat);
        tempRT.wrapMode = TextureWrapMode.Repeat;

        // Add external force
        Graphics.Blit(m_velocityRT, tempRT);
        m_material.SetTexture("_VelocityTex", tempRT);
        Graphics.Blit((RenderTexture)null, m_velocityRT, m_material, 5);

        // Advection
        Graphics.Blit(m_velocityRT, tempRT);
        m_material.SetTexture("_VelocityTex", tempRT);
        m_material.SetTexture("_PressureTex", m_pressureRT);
        Graphics.Blit(tempRT, m_velocityRT, m_material, 2);

        // Set boundary conditions
        //Graphics.Blit(m_velocityRT, tempRT);
        //m_material.SetTexture("_VelocityTex", tempRT);
        //Graphics.Blit((RenderTexture)null, m_velocityRT, m_material, 6);

        // Viscosity
        Graphics.Blit(m_velocityRT, tempRT);
        m_material.SetTexture("_VelocityTex", tempRT);
        Graphics.Blit((RenderTexture)null, m_velocityRT, m_material, 1);

        // Solve Poisson
        m_material.SetTexture("_VelocityTex", m_velocityRT);
        for (int i = 0; i < 8; ++i)
        {
            Graphics.Blit(m_pressureRT, tempRT);
            m_material.SetTexture("_PressureTex", tempRT);
            Graphics.Blit((RenderTexture)null, m_pressureRT, m_material, 3);
        }

        // Projection
        Graphics.Blit(m_velocityRT, tempRT);
        m_material.SetTexture("_VelocityTex", tempRT);
        m_material.SetTexture("_PressureTex", m_pressureRT);
        Graphics.Blit((RenderTexture)null, m_velocityRT, m_material, 4);

        // Image Advection
        m_material.SetTexture("_VelocityTex", m_velocityRT);
        Graphics.Blit(m_imageRT, tempRT);
        Graphics.Blit(tempRT, m_imageRT, m_material, 2);

        Graphics.Blit(m_velocityRT, viewRT, m_tex2ViewMat, 0);

        RenderTexture.ReleaseTemporary(tempRT);
    }

    void Update()
    {
        m_material.SetTexture("_PrevImage", m_prevImage);
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_material, 6);
    }

    void OnGUI()
    {
        int texSize = Screen.height / 2;

        GUI.DrawTexture(new Rect(0, 0, texSize, texSize), viewRT);
        // GUI.DrawTexture(new Rect(texSize, 0, texSize, texSize), m_prevImage);
    }
}
