using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ScreenSpaceJointVisualizer : MonoBehaviour
    {
        
        enum JointIndices
        {
            Invalid = -1,
            Head = 0, 
            Neck1 = 1, 
            RightShoulder1 = 2, 
            RightForearm = 3, 
            RightHand = 4, 
            LeftShoulder1 = 5, 
            LeftForearm = 6, 
            LeftHand = 7, 
            RightUpLeg = 8, 
            RightLeg = 9, 
            RightFoot = 10, 
            LeftUpLeg = 11, 
            LeftLeg = 12, 
            LeftFoot = 13, 
            RightEye = 14, 
            LeftEye = 15, 
            Root = 16, 
            RightEar = 17, 
            LeftEar = 18 
        }

        [SerializeField]
        Camera m_ARCamera; 
        public Camera arCamera
        {
            get { return m_ARCamera; }
            set { m_ARCamera = value; }
        }

        [SerializeField]
        ARHumanBodyManager m_HumanBodyManager;
        public ARHumanBodyManager humanBodyManager
        {
            get { return m_HumanBodyManager; }
            set { m_HumanBodyManager = value; }
        }

        [SerializeField]
        GameObject m_LineRendererPrefab;
        public GameObject lineRendererPrefab
        {
            get { return m_LineRendererPrefab; }
            set { m_LineRendererPrefab = value; }
        }

        Dictionary<int, GameObject> m_LineRenderers = new();
        static HashSet<int> s_JointSet = new();

        void UpdateRenderer(NativeArray<XRHumanBodyPose2DJoint> joints, int index)
        {
            if (!m_LineRenderers.TryGetValue(index, out var lineRendererGO))
            {
                lineRendererGO = Instantiate(m_LineRendererPrefab, transform);
                m_LineRenderers.Add(index, lineRendererGO);
            }

            var lineRenderer = lineRendererGO.GetComponent<LineRenderer>();

            
            var positions = new NativeArray<Vector2>(joints.Length, Allocator.Temp);
            try
            {
                var boneIndex = index;
                var jointCount = 0;
                while (boneIndex >= 0)
                {
                    var joint = joints[boneIndex];
                    if (joint.tracked)
                    {
                        positions[jointCount++] = joint.position;
                        if (!s_JointSet.Add(boneIndex))
                            break;
                    }
                    else
                        break;

                    boneIndex = joint.parentIndex;
                }

                
                lineRenderer.positionCount = jointCount;
                lineRenderer.startWidth = 0.001f;
                lineRenderer.endWidth = 0.001f;
                for (var i = 0; i < jointCount; ++i)
                {
                    var position = positions[i];
                    var worldPosition = m_ARCamera.ViewportToWorldPoint(
                        new Vector3(position.x, position.y, m_ARCamera.nearClipPlane));
                    lineRenderer.SetPosition(i, worldPosition);
                }
                lineRendererGO.SetActive(true);
            }
            finally
            {
                positions.Dispose();
            }
        }

        void Update()
        {
            var joints = m_HumanBodyManager.GetHumanBodyPose2DJoints(Allocator.Temp);
            if (!joints.IsCreated)
            {
                HideJointLines();
                return;
            }

            using (joints)
            {
                s_JointSet.Clear();
                for (var i = joints.Length - 1; i >= 0; --i)
                {
                    if (joints[i].parentIndex != -1)
                        UpdateRenderer(joints, i);
                }
            }
        }

        void HideJointLines()
        {
            foreach (var lineRenderer in m_LineRenderers)
            {
                lineRenderer.Value.SetActive(false);
            }
        }
    }
}
