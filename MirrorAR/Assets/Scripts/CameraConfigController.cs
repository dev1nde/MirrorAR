using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(Dropdown))]
    public class CameraConfigController : MonoBehaviour
    {
        List<string> m_ConfigurationNames;

        Dropdown m_Dropdown;

        [SerializeField]
        [Tooltip("The ARCameraManager which will produce frame events.")]
        ARCameraManager m_CameraManager;
        public ARCameraManager cameraManager
        {
            get => m_CameraManager;
            set => m_CameraManager = value;
        }
        void OnDropdownValueChanged(Dropdown dropdown)
        {
            if ((cameraManager == null) || (cameraManager.subsystem == null) || !cameraManager.subsystem.running)
            {
                return;
            }

            var configurationIndex = dropdown.value;

            using (var configurations = cameraManager.GetConfigurations(Allocator.Temp))
            {
                if (configurationIndex >= configurations.Length)
                {
                    return;
                }

                var configuration = configurations[configurationIndex];
                cameraManager.currentConfiguration = configuration;
            }
        }

        void Awake()
        {
            m_Dropdown = GetComponent<Dropdown>();
            m_Dropdown.ClearOptions();
            m_Dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(m_Dropdown); });
            m_ConfigurationNames = new List<string>();
        }

        void PopulateDropdown()
        {
            if ((cameraManager == null) || (cameraManager.subsystem == null) || !cameraManager.subsystem.running)
                return;

            using (var configurations = cameraManager.GetConfigurations(Allocator.Temp))
            {
                if (!configurations.IsCreated || (configurations.Length <= 0))
                {
                    return;
                }

                foreach (var config in configurations)
                {
                    m_ConfigurationNames.Add($"{config.width}x{config.height}{(config.framerate.HasValue ? $" at {config.framerate.Value} Hz" : "")}{(config.depthSensorSupported == Supported.Supported ? " depth sensor" : "")}");
                }
                m_Dropdown.AddOptions(m_ConfigurationNames);

                var currentConfig = cameraManager.currentConfiguration;
                for (int i = 0; i < configurations.Length; ++i)
                {
                    if (currentConfig == configurations[i])
                    {
                        m_Dropdown.value = i;
                    }
                }
            }
        }

        void Update()
        {
            if (m_ConfigurationNames.Count == 0)
                PopulateDropdown();
        }
    }
}
