using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.Rendering.Universal; // requires URP + 2D Renderer for Light2D
#endif

namespace ShadowEscape.Environment
{
    /// <summary>
    /// Subtly flickers a Light2D's intensity to create a moody, dynamic-lighting feel
    /// (torches, lanterns, exit glow, etc.). Requires the Universal Render Pipeline
    /// with the 2D Renderer enabled and a Light2D component on this object.
    /// Attach to: any GameObject with a Light2D component.
    /// </summary>
    public class LightFlicker2D : MonoBehaviour
    {
        [SerializeField] private float baseIntensity = 1f;
        [SerializeField] private float flickerAmount = 0.15f;
        [SerializeField] private float flickerSpeed = 8f;

#if UNITY_2019_1_OR_NEWER
        private Light2D light2D;

        private void Awake()
        {
            light2D = GetComponent<Light2D>();
        }

        private void Update()
        {
            if (light2D == null) return;
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) - 0.5f;
            light2D.intensity = baseIntensity + noise * flickerAmount;
        }
#endif
    }
}
