using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Light))]
    public class HDRLightEstimation : MonoBehaviour
    {
        [SerializeField]
        ARCameraManager m_CameraManager;

        [SerializeField]
        Transform m_Arrow;

        public Transform arrow
        {
            get => m_Arrow;
            set => m_Arrow = value;
        }
        public ARCameraManager cameraManager
        {
            get { return m_CameraManager; }
            set
            {
                if (m_CameraManager == value)
                    return;

                if (m_CameraManager != null)
                    m_CameraManager.frameReceived -= FrameChanged;

                m_CameraManager = value;

                if (m_CameraManager != null & enabled)
                    m_CameraManager.frameReceived += FrameChanged;
            }
        }
        public float? brightness { get; private set; }
        public float? colorTemperature { get; private set; }
        public Color? colorCorrection { get; private set; }
        public Vector3? mainLightDirection { get; private set; }
        public Color? mainLightColor { get; private set; }
        public float? mainLightIntensityLumens { get; private set; }
        public SphericalHarmonicsL2? sphericalHarmonics { get; private set; }
        void Awake ()
        {
            m_Light = GetComponent<Light>();
        }

        void OnEnable()
        {
            if (m_CameraManager != null)
                m_CameraManager.frameReceived += FrameChanged;

            
            if (arrow)
            {
                arrow.gameObject.SetActive(false);
            }
            Application.onBeforeRender += OnBeforeRender;
        }

        void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;

            if (m_CameraManager != null)
                m_CameraManager.frameReceived -= FrameChanged;
        }

        void OnBeforeRender()
        {
            if (arrow && m_CameraManager)
            {
                var cameraTransform = m_CameraManager.GetComponent<Camera>().transform;
                arrow.position = cameraTransform.position + cameraTransform.forward * .25f;
            }
        }

        void FrameChanged(ARCameraFrameEventArgs args)
        {
            if (args.lightEstimation.averageBrightness.HasValue)
            {
                brightness = args.lightEstimation.averageBrightness.Value;
                m_Light.intensity = brightness.Value;
            }
            else
            {
                brightness = null;
            }

            if (args.lightEstimation.averageColorTemperature.HasValue)
            {
                colorTemperature = args.lightEstimation.averageColorTemperature.Value;
                m_Light.colorTemperature = colorTemperature.Value;
            }
            else
            {
                colorTemperature = null;
            }

            if (args.lightEstimation.colorCorrection.HasValue)
            {
                colorCorrection = args.lightEstimation.colorCorrection.Value;
                m_Light.color = colorCorrection.Value;
            }
            else
            {
                colorCorrection = null;
            }
            
            if (args.lightEstimation.mainLightDirection.HasValue)
            {
                mainLightDirection = args.lightEstimation.mainLightDirection;
                m_Light.transform.rotation = Quaternion.LookRotation(mainLightDirection.Value);
                if (arrow)
                {
                    arrow.gameObject.SetActive(true);
                    arrow.rotation = Quaternion.LookRotation(mainLightDirection.Value);
                }
            }
            else if (arrow)
            {
                arrow.gameObject.SetActive(false);
                mainLightDirection = null;
            }

            if (args.lightEstimation.mainLightColor.HasValue)
            {
                mainLightColor = args.lightEstimation.mainLightColor;
                m_Light.color = mainLightColor.Value;
            }
            else
            {
                mainLightColor = null;
            }

            if (args.lightEstimation.mainLightIntensityLumens.HasValue)
            {
                mainLightIntensityLumens = args.lightEstimation.mainLightIntensityLumens;
                m_Light.intensity = args.lightEstimation.averageMainLightBrightness.Value;
            }
            else
            {
                mainLightIntensityLumens = null;
            }

            if (args.lightEstimation.ambientSphericalHarmonics.HasValue)
            {
                sphericalHarmonics = args.lightEstimation.ambientSphericalHarmonics;
                RenderSettings.ambientMode = AmbientMode.Skybox;
                RenderSettings.ambientProbe = sphericalHarmonics.Value;
            }
            else
            {
                sphericalHarmonics = null;
            }
        }

        Light m_Light;
    }
}
