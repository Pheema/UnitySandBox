using UnityEngine;
using System.Collections;

namespace Ph.Effects
{
    public class Audio2Glitch : MonoBehaviour
    {
        ScreenGlitchController m_screenGlitchController;
        float[] data = new float[256];

        [Header("InputSettings")]
        [Range(-1f, 0f)]
        public float offset = -0.1f;
        public float audioGain = 1.0f;

        [Header("OutputSettings")]
        public float freqCoief = 1.0f;
        public float ampCoief = 1.0f;

        void Start()
        {
            m_screenGlitchController = GetComponent<ScreenGlitchController>();
        }

        void Update()
        {
            AudioListener.GetOutputData(data, 0);
            float rms = 0f;
            for (int i = 0; i < data.Length; ++i)
            {
                rms += data[i] * data[i];
            }
            rms = Mathf.Sqrt(rms / data.Length);

            rms = Mathf.Clamp01((rms + offset) * audioGain);
            rms = Mathf.SmoothStep(0f, 1f, rms);
            rms = Mathf.SmoothStep(0f, 1f, rms);

            m_screenGlitchController.frequency = freqCoief * rms;
            m_screenGlitchController.amplitude = ampCoief * rms;
        }
    }
}