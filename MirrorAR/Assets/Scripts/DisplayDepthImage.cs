using System.Text;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class DisplayDepthImage : MonoBehaviour
    {
        enum DisplayMode
        {
            EnvironmentDepthRaw = 0,
            EnvironmentDepthSmooth = 1,
            HumanDepth = 2,
            HumanStencil = 3,
        }
        const string k_MaxDistanceName = "_MaxDistance";
        const string k_DisplayRotationPerFrameName = "_DisplayRotationPerFrame";
        const float k_DefaultTextureAspectRadio = 1.0f;
        static readonly int k_MaxDistanceId = Shader.PropertyToID(k_MaxDistanceName); 
        static readonly int k_DisplayRotationPerFrameId = Shader.PropertyToID(k_DisplayRotationPerFrameName);
        readonly StringBuilder m_StringBuilder = new StringBuilder();
        ScreenOrientation m_CurrentScreenOrientation;
        float m_TextureAspectRatio = k_DefaultTextureAspectRadio;
        DisplayMode m_DisplayMode = DisplayMode.EnvironmentDepthRaw;
        Matrix4x4 m_DisplayRotationMatrix = Matrix4x4.identity;
#if UNITY_ANDROID
        Matrix4x4 k_AndroidFlipYMatrix = Matrix4x4.identity;
#endif  
 
        public AROcclusionManager occlusionManager
        {
            get => m_OcclusionManager;
            set => m_OcclusionManager = value;
        }
        [SerializeField]
        AROcclusionManager m_OcclusionManager;
        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }

        [SerializeField]
        ARCameraManager m_CameraManager;
        public RawImage rawImage
        {
            get => m_RawImage;
            set => m_RawImage = value;
        }

        [SerializeField]
        RawImage m_RawImage;
        public Text imageInfo
        {
            get => m_ImageInfo;
            set => m_ImageInfo = value;
        }

        [SerializeField]
        Text m_ImageInfo;
        public Material depthMaterial
        {
            get => m_DepthMaterial;
            set => m_DepthMaterial = value;
        }
        [SerializeField]
        Material m_DepthMaterial;
        public Material stencilMaterial
        {
            get => m_StencilMaterial;
            set => m_StencilMaterial = value;
        }
        [SerializeField]
        Material m_StencilMaterial;
        public float maxEnvironmentDistance
        {
            get => m_MaxEnvironmentDistance;
            set => m_MaxEnvironmentDistance = value;
        }

        [SerializeField]
        float m_MaxEnvironmentDistance = 8.0f;
        public float maxHumanDistance
        {
            get => m_MaxHumanDistance;
            set => m_MaxHumanDistance = value;
        }

        [SerializeField]
        float m_MaxHumanDistance = 3.0f;

        void Awake()
        {
#if UNITY_ANDROID
            k_AndroidFlipYMatrix[1,1] = -1.0f;
            k_AndroidFlipYMatrix[2,1] = 1.0f;
#endif  
        }

        void OnEnable()
        {
             
            Debug.Assert(m_CameraManager != null, "no camera manager");
            m_CameraManager.frameReceived += OnCameraFrameEventReceived;
            m_DisplayRotationMatrix = Matrix4x4.identity;

             
            m_CurrentScreenOrientation = Screen.orientation;
            UpdateRawImage();
        }

        void OnDisable()
        {
             
            Debug.Assert(m_CameraManager != null, "no camera manager");
            m_CameraManager.frameReceived -= OnCameraFrameEventReceived;
            m_DisplayRotationMatrix = Matrix4x4.identity;
        }

        void Update()
        {
             
             
            Debug.Assert(m_OcclusionManager != null, "no occlusion manager");

            var descriptor = m_OcclusionManager.descriptor;
            switch (m_DisplayMode)
            {
                case DisplayMode.HumanDepth:
                case DisplayMode.HumanStencil:
                {
                    if (descriptor != null &&
                        (descriptor.humanSegmentationDepthImageSupported == Supported.Supported ||
                        descriptor.humanSegmentationStencilImageSupported == Supported.Supported))
                    {
                        break;
                    }

                    if (descriptor != null &&
                        (descriptor.humanSegmentationStencilImageSupported == Supported.Unknown ||
                         descriptor.humanSegmentationDepthImageSupported == Supported.Unknown))
                    {
                        LogText("Determining human segmentation support...");
                    }
                    else
                    {
                        LogText("Human segmentation is not supported on this device.");
                    }

                    m_RawImage.texture = null;
                    if (!Mathf.Approximately(m_TextureAspectRatio, k_DefaultTextureAspectRadio))
                    {
                        m_TextureAspectRatio = k_DefaultTextureAspectRadio;
                        UpdateRawImage();
                    }

                    return;
                }
                case DisplayMode.EnvironmentDepthRaw:
                case DisplayMode.EnvironmentDepthSmooth:
                default:
                {
                    if (descriptor.environmentDepthImageSupported == Supported.Supported)
                    {
                        m_OcclusionManager.environmentDepthTemporalSmoothingRequested = m_DisplayMode == DisplayMode.EnvironmentDepthSmooth;
                        break;
                    }

                    m_RawImage.texture = null;
                    if (!Mathf.Approximately(m_TextureAspectRatio, k_DefaultTextureAspectRadio))
                    {
                        m_TextureAspectRatio = k_DefaultTextureAspectRadio;
                        UpdateRawImage();
                    }

                    return;
                }
            }

            Texture2D humanStencil = m_OcclusionManager.humanStencilTexture;
            Texture2D humanDepth = m_OcclusionManager.humanDepthTexture;
            Texture2D envDepth = m_OcclusionManager.environmentDepthTexture;

             
            m_StringBuilder.Clear();
            BuildTextureInfo(m_StringBuilder, "stencil", humanStencil);
            BuildTextureInfo(m_StringBuilder, "depth", humanDepth);
            BuildTextureInfo(m_StringBuilder, "env", envDepth);

            LogText(m_StringBuilder.ToString());

             
            Texture2D displayTexture;
            switch (m_DisplayMode)
            {
                case DisplayMode.HumanStencil:
                    displayTexture = humanStencil;
                    break;
                case DisplayMode.HumanDepth:
                    displayTexture = humanDepth;
                    break;
                case DisplayMode.EnvironmentDepthRaw:
                case DisplayMode.EnvironmentDepthSmooth:
                default:
                    displayTexture = envDepth;
                    break;
            }

            Debug.Assert(m_RawImage != null, "no raw image");
            m_RawImage.texture = displayTexture;
             
            float textureAspectRatio = (displayTexture == null) ? 1.0f : ((float)displayTexture.width / (float)displayTexture.height);
             
            if ((m_CurrentScreenOrientation != Screen.orientation)
                || !Mathf.Approximately(m_TextureAspectRatio, textureAspectRatio))
            {
                m_CurrentScreenOrientation = Screen.orientation;
                m_TextureAspectRatio = textureAspectRatio;
                UpdateRawImage();
            }
        }   
        void OnCameraFrameEventReceived(ARCameraFrameEventArgs cameraFrameEventArgs)
        {
            Debug.Assert(m_RawImage != null, "no raw image");
            if (m_RawImage.material != null)
            {
                 
                Matrix4x4 cameraMatrix = cameraFrameEventArgs.displayMatrix ?? Matrix4x4.identity;

                Vector2 affineBasisX = new Vector2(1.0f, 0.0f);
                Vector2 affineBasisY = new Vector2(0.0f, 1.0f);
                Vector2 affineTranslation = new Vector2(0.0f, 0.0f);
#if UNITY_IOS
                affineBasisX = new Vector2(cameraMatrix[0, 0], cameraMatrix[1, 0]);
                affineBasisY = new Vector2(cameraMatrix[0, 1], cameraMatrix[1, 1]);
                affineTranslation = new Vector2(cameraMatrix[2, 0], cameraMatrix[2, 1]);
#endif  
#if UNITY_ANDROID
                affineBasisX = new Vector2(cameraMatrix[0, 0], cameraMatrix[0, 1]);
                affineBasisY = new Vector2(cameraMatrix[1, 0], cameraMatrix[1, 1]);
                affineTranslation = new Vector2(cameraMatrix[0, 2], cameraMatrix[1, 2]);
#endif     
                 
                affineBasisX = affineBasisX.normalized;
                affineBasisY = affineBasisY.normalized;
                m_DisplayRotationMatrix = Matrix4x4.identity;
                m_DisplayRotationMatrix[0,0] = affineBasisX.x;
                m_DisplayRotationMatrix[0,1] = affineBasisY.x;
                m_DisplayRotationMatrix[1,0] = affineBasisX.y;
                m_DisplayRotationMatrix[1,1] = affineBasisY.y;
                m_DisplayRotationMatrix[2,0] = Mathf.Round(affineTranslation.x);
                m_DisplayRotationMatrix[2,1] = Mathf.Round(affineTranslation.y);

#if UNITY_ANDROID
                m_DisplayRotationMatrix = k_AndroidFlipYMatrix * m_DisplayRotationMatrix;
#endif  

                 
                m_RawImage.material.SetMatrix(k_DisplayRotationPerFrameId, m_DisplayRotationMatrix);
            }
        }
         
        void BuildTextureInfo(StringBuilder stringBuilder, string textureName, Texture2D texture)
        {
            stringBuilder.AppendLine($"texture : {textureName}");
            if (texture == null)
            {
                stringBuilder.AppendLine("   <null>");
            }
            else
            {
                stringBuilder.AppendLine($"   format : {texture.format}");
                stringBuilder.AppendLine($"   width  : {texture.width}");
                stringBuilder.AppendLine($"   height : {texture.height}");
                stringBuilder.AppendLine($"   mipmap : {texture.mipmapCount}");
            }
        }
         
        void LogText(string text)
        {
            if (m_ImageInfo != null)
            {
                m_ImageInfo.text = text;
            }
            else
            {
                Debug.Log(text);
            }
        }

        void UpdateRawImage()
        {
            Debug.Assert(m_RawImage != null, "no raw image");

             
             
            float minDimension = 480.0f;
            float maxDimension = Mathf.Round(minDimension * m_TextureAspectRatio);
            Vector2 rectSize;
            switch (m_CurrentScreenOrientation)
            {
                case ScreenOrientation.LandscapeRight:
                case ScreenOrientation.LandscapeLeft:
                    rectSize = new Vector2(maxDimension, minDimension);
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                case ScreenOrientation.Portrait:
                default:
                    rectSize = new Vector2(minDimension, maxDimension);
                    break;
            }

             
            float maxDistance;
            Material material;
            switch (m_DisplayMode)
            {
                case DisplayMode.HumanStencil:
                    material = m_StencilMaterial;
                    maxDistance = m_MaxHumanDistance;
                    break;
                case DisplayMode.HumanDepth:
                    material = m_DepthMaterial;
                    maxDistance = m_MaxHumanDistance;
                    break;
                case DisplayMode.EnvironmentDepthRaw:
                case DisplayMode.EnvironmentDepthSmooth:
                default:
                    material = m_DepthMaterial;
                    maxDistance = m_MaxEnvironmentDistance;
                    break;
            }

             
            m_RawImage.rectTransform.sizeDelta = rectSize;
            material.SetFloat(k_MaxDistanceId, maxDistance);
            material.SetMatrix(k_DisplayRotationPerFrameId, m_DisplayRotationMatrix);
            m_RawImage.material = material;
        }

        public void OnDepthModeDropdownValueChanged(Dropdown dropdown)
        {      
            m_DisplayMode = (DisplayMode)dropdown.value;
            UpdateRawImage();
        }
    }
}
