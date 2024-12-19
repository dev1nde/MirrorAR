using UnityEngine.XR.ARFoundation;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class BoneController : MonoBehaviour
    {
        enum JointIndices
        {
            Invalid = -1,
            Root = 0,  
            Hips = 1,  
            LeftUpLeg = 2,  
            LeftLeg = 3,  
            LeftFoot = 4,  
            LeftToes = 5,  
            LeftToesEnd = 6,  
            RightUpLeg = 7,  
            RightLeg = 8,  
            RightFoot = 9,  
            RightToes = 10,  
            RightToesEnd = 11,  
            Spine1 = 12,  
            Spine2 = 13,  
            Spine3 = 14,  
            Spine4 = 15,  
            Spine5 = 16,  
            Spine6 = 17,  
            Spine7 = 18,  
            LeftShoulder1 = 19,  
            LeftArm = 20,  
            LeftForearm = 21,  
            LeftHand = 22,  
            LeftHandIndexStart = 23,  
            LeftHandIndex1 = 24,  
            LeftHandIndex2 = 25,  
            LeftHandIndex3 = 26,  
            LeftHandIndexEnd = 27,  
            LeftHandMidStart = 28,  
            LeftHandMid1 = 29,  
            LeftHandMid2 = 30,  
            LeftHandMid3 = 31,  
            LeftHandMidEnd = 32,  
            LeftHandPinkyStart = 33,  
            LeftHandPinky1 = 34,  
            LeftHandPinky2 = 35,  
            LeftHandPinky3 = 36,  
            LeftHandPinkyEnd = 37,  
            LeftHandRingStart = 38,  
            LeftHandRing1 = 39,  
            LeftHandRing2 = 40,  
            LeftHandRing3 = 41,  
            LeftHandRingEnd = 42,  
            LeftHandThumbStart = 43,  
            LeftHandThumb1 = 44,  
            LeftHandThumb2 = 45,  
            LeftHandThumbEnd = 46,  
            Neck1 = 47,  
            Neck2 = 48,  
            Neck3 = 49,  
            Neck4 = 50,  
            Head = 51,  
            Jaw = 52,  
            Chin = 53,  
            LeftEye = 54,  
            LeftEyeLowerLid = 55,  
            LeftEyeUpperLid = 56,  
            LeftEyeball = 57,  
            Nose = 58,  
            RightEye = 59,  
            RightEyeLowerLid = 60,  
            RightEyeUpperLid = 61,  
            RightEyeball = 62,  
            RightShoulder1 = 63,  
            RightArm = 64,  
            RightForearm = 65,  
            RightHand = 66,  
            RightHandIndexStart = 67,  
            RightHandIndex1 = 68,  
            RightHandIndex2 = 69,  
            RightHandIndex3 = 70,  
            RightHandIndexEnd = 71,  
            RightHandMidStart = 72,  
            RightHandMid1 = 73,  
            RightHandMid2 = 74,  
            RightHandMid3 = 75,  
            RightHandMidEnd = 76,  
            RightHandPinkyStart = 77,  
            RightHandPinky1 = 78,  
            RightHandPinky2 = 79,  
            RightHandPinky3 = 80,  
            RightHandPinkyEnd = 81,  
            RightHandRingStart = 82,  
            RightHandRing1 = 83,  
            RightHandRing2 = 84,  
            RightHandRing3 = 85,  
            RightHandRingEnd = 86,  
            RightHandThumbStart = 87,  
            RightHandThumb1 = 88,  
            RightHandThumb2 = 89,  
            RightHandThumbEnd = 90,  
        }
        const int k_NumSkeletonJoints = 91;

        [SerializeField]
        Transform m_SkeletonRoot;

        public Transform skeletonRoot
        {
            get
            {
                return m_SkeletonRoot;
            }
            set
            {
                m_SkeletonRoot = value;
            }
        }

        Transform[] m_BoneMapping = new Transform[k_NumSkeletonJoints];

        public void InitializeSkeletonJoints()
        {
            Queue<Transform> nodes = new Queue<Transform>();
            nodes.Enqueue(m_SkeletonRoot);
            while (nodes.Count > 0)
            {
                Transform next = nodes.Dequeue();
                for (int i = 0; i < next.childCount; ++i)
                {
                    nodes.Enqueue(next.GetChild(i));
                }
                ProcessJoint(next);
            }
        }

        public void ApplyBodyPose(ARHumanBody body)
        {
            var joints = body.joints;
            if (!joints.IsCreated)
                return;

            for (int i = 0; i < k_NumSkeletonJoints; ++i)
            {
                XRHumanBodyJoint joint = joints[i];
                var bone = m_BoneMapping[i];
                if (bone != null)
                {
                    bone.transform.localPosition = joint.localPose.position;
                    bone.transform.localRotation = joint.localPose.rotation;
                }
            }
        }

        void ProcessJoint(Transform joint)
        {
            int index = GetJointIndex(joint.name);
            if (index >= 0 && index < k_NumSkeletonJoints)
            {
                m_BoneMapping[index] = joint;
            }
        }

        int GetJointIndex(string jointName)
        {
            JointIndices val;
            if (Enum.TryParse(jointName, out val))
            {
                return (int)val;
            }
            return -1;
        }
    }
}