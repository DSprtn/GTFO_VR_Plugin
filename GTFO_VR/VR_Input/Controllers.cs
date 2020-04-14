using GTFO_VR.Core;
using GTFO_VR.Events;
using GTFO_VR.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Input
{
    public class Controllers : MonoBehaviourExtended
    {

        public static GameObject leftController;

        public static GameObject rightController;

        static GameObject mainController;

        static GameObject offhandController;

        void Awake()
        {
            SetupControllers();
            SetMainController();
        }

        private void SetMainController()
        {
            if (VRSettings.mainHand.Equals(HandType.Right))
            {
                mainController = rightController;
                offhandController = leftController;
            }
            else
            {
                mainController = leftController;
                offhandController = rightController;
            }
        }

        public static Transform GetNonMainControllerTransform()
        {
            if(rightController.Equals(mainController))
            {
                return leftController.transform;
            }
            return rightController.transform;
        }

        private void SetupControllers()
        {
            leftController = SetupController(SteamVR_Input_Sources.LeftHand);
            rightController = SetupController(SteamVR_Input_Sources.RightHand);
            leftController.name = "LeftController";
            rightController.name = "RightController";

            DontDestroyOnLoad(rightController);
            DontDestroyOnLoad(leftController);
        }

        public static void SetOrigin(Transform origin)
        {
            leftController.transform.SetParent(origin);
            rightController.transform.SetParent(origin);
        }

        GameObject SetupController(SteamVR_Input_Sources source)
        {
            GameObject controller = new GameObject("Controller");
            SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = controller.AddComponent<SteamVR_Behaviour_Pose>();
            steamVR_Behaviour_Pose.inputSource = source;
            steamVR_Behaviour_Pose.broadcastDeviceChanges = true;
            return controller;
        }

        public static  Vector3 GetAimForward()
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                return ItemEquippableEvents.currentItem.MuzzleAlign.forward;
            }
            if (!mainController)
            {
                return HMD.hmd.transform.forward;
            }
            return (mainController.transform.rotation * Vector3.forward);
        }

        public static Vector3 GetTwoHandedAimForward()
        {
            float currentItemYOffset = 0f;
            Vector3 offhandPos = offhandController.transform.position;
            offhandPos.y += currentItemYOffset;
            return (offhandPos - mainController.transform.position).normalized;
        }

        public static Vector3 GetTwoHandedTransformUp()
        {
            return (mainController.transform.up + offhandController.transform.up) / 2;
        }

        public static Quaternion GetTwoHandedRotation()
        {
            return Quaternion.LookRotation(GetTwoHandedAimForward());
        }

        public static Vector3 GetTwoHandedPos()
        {
            return (mainController.transform.position + offhandController.transform.position) / 2;
        }

        public static Vector3 GetAimFromPos()
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                return ItemEquippableEvents.currentItem.MuzzleAlign.position;
            }
            if (!mainController)
            {
                return HMD.hmd.transform.position;
            }
            return mainController.transform.position;
        }

        public static Quaternion GetAimFromRot()
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                return ItemEquippableEvents.currentItem.MuzzleAlign.rotation;
            }
            if (!mainController)
            {
                return Quaternion.identity;
            }
            return mainController.transform.rotation;
        }

        public static Quaternion GetControllerAimRotation()
        {
            if (!mainController)
            {
                return Quaternion.identity;
            }
            return mainController.transform.rotation;
        }

        public static Vector3 GetControllerPosition()
        {
            if (!mainController)
            {
                return Vector3.zero;
            }
            return mainController.transform.position;
        }
    }
}
