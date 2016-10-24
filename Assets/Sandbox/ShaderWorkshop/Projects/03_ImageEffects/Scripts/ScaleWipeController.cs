using UnityEngine;

namespace Ph.Effects
{
    public class ScaleWipeController : MonoBehaviour
    {
        AudioLowPassFilter m_audioLowPassFilter;
        ScaleWipe m_scaleWipe;

        public float radius = 0.6f;
        public float freq = 5000f;
        public float fov = 8f;

        float m_fov0;

        float wipeRadius = 1.0f;
        float cutOffFreq = 22000f;
        float currentFov;

        float vel = 0.0f;
        float velFreq = 0.0f;
        float velFov = 0.0f;

        void Start()
        {
            currentFov = fov;

            m_fov0 = Camera.main.fieldOfView;
            m_scaleWipe = GetComponent<ScaleWipe>();
            m_audioLowPassFilter = GetComponent<AudioLowPassFilter>();
        }

        void Update()
        {
            bool key = Input.GetKey(KeyCode.Space);
            float targetVal = key ? radius : 1f;
            float targetCutOffFreq = key ? freq : 22000f;
            float targetFov = key ? fov : m_fov0;

            wipeRadius = Mathf.SmoothDamp(wipeRadius, targetVal, ref vel, 0.1f);
            cutOffFreq = Mathf.SmoothDamp(cutOffFreq, targetCutOffFreq, ref velFreq, 0.1f);
            currentFov = Mathf.SmoothDamp(currentFov, targetFov, ref velFov, 0.1f);

            m_scaleWipe.radius = wipeRadius;
            m_audioLowPassFilter.cutoffFrequency = cutOffFreq;
            Camera.main.fieldOfView = currentFov;
        }
    }
}