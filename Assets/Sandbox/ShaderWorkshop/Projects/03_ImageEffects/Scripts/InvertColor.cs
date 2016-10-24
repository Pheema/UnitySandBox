using UnityEngine;

[ExecuteInEditMode]
#if UNITY_5_4_OR_NEWER
[ImageEffectAllowedInSceneView]
#endif
public class InvertColor : MonoBehaviour
{
    [SerializeField]
    Shader m_shader;

    Material m_material;
    public Material material
    {
        get
        {
            if (m_material == null)
            {
                m_material = new Material(m_shader);
                m_material.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_material;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }

    void OnDisable()
    {
        if (m_material != null) DestroyImmediate(m_material);
    }
}
