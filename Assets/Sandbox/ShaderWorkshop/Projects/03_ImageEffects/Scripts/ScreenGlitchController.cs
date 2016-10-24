using UnityEngine;
using System.Collections;

namespace Ph.Effects
{
    [RequireComponent(typeof(ScreenGlitch))]
    public class ScreenGlitchController : MonoBehaviour
    {
        ScreenGlitch m_screenGlitch;

        [Range(0f, 1f)]
        public float frequency = 0.1f;

        public float amplitude = 0.1f;

        void Start()
        {
            m_screenGlitch = GetComponent<ScreenGlitch>();
        }

        void Update()
        {
            float val = (Random.value < frequency) ? amplitude : 0f;
            m_screenGlitch.amplitude = val;
            m_screenGlitch.seed = Random.Range(0, 1024);
        }
    }
}
