using UnityEngine;
using Bhaptics.Tact;
using GTFO_VR.Events;
using System;

namespace GTFO_VR.Core.PlayerBehaviours
{
    public class BhapticsIntegration : MonoBehaviour
    {
        private static readonly string FIRE_KEY = "fire";
        private static readonly string EXPLOSION_HAPTIC_STR =
            "{\"project\":{\"createdAt\":1593681836617,\"description\":\"\",\"layout\":{\"layouts\":{\"VestBack\":[{\"index\":0,\"x\":0,\"y\":0},{\"index\":1,\"x\":0.333,\"y\":0},{\"index\":2,\"x\":0.667,\"y\":0},{\"index\":3,\"x\":1,\"y\":0},{\"index\":4,\"x\":0,\"y\":0.25},{\"index\":5,\"x\":0.333,\"y\":0.25},{\"index\":6,\"x\":0.667,\"y\":0.25},{\"index\":7,\"x\":1,\"y\":0.25},{\"index\":8,\"x\":0,\"y\":0.5},{\"index\":9,\"x\":0.333,\"y\":0.5},{\"index\":10,\"x\":0.667,\"y\":0.5},{\"index\":11,\"x\":1,\"y\":0.5},{\"index\":12,\"x\":0,\"y\":0.75},{\"index\":13,\"x\":0.333,\"y\":0.75},{\"index\":14,\"x\":0.667,\"y\":0.75},{\"index\":15,\"x\":1,\"y\":0.75},{\"index\":16,\"x\":0,\"y\":1},{\"index\":17,\"x\":0.333,\"y\":1},{\"index\":18,\"x\":0.667,\"y\":1},{\"index\":19,\"x\":1,\"y\":1}],\"VestFront\":[{\"index\":0,\"x\":0,\"y\":0},{\"index\":1,\"x\":0.333,\"y\":0},{\"index\":2,\"x\":0.667,\"y\":0},{\"index\":3,\"x\":1,\"y\":0},{\"index\":4,\"x\":0,\"y\":0.25},{\"index\":5,\"x\":0.333,\"y\":0.25},{\"index\":6,\"x\":0.667,\"y\":0.25},{\"index\":7,\"x\":1,\"y\":0.25},{\"index\":8,\"x\":0,\"y\":0.5},{\"index\":9,\"x\":0.333,\"y\":0.5},{\"index\":10,\"x\":0.667,\"y\":0.5},{\"index\":11,\"x\":1,\"y\":0.5},{\"index\":12,\"x\":0,\"y\":0.75},{\"index\":13,\"x\":0.333,\"y\":0.75},{\"index\":14,\"x\":0.667,\"y\":0.75},{\"index\":15,\"x\":1,\"y\":0.75},{\"index\":16,\"x\":0,\"y\":1},{\"index\":17,\"x\":0.333,\"y\":1},{\"index\":18,\"x\":0.667,\"y\":1},{\"index\":19,\"x\":1,\"y\":1}]},\"name\":\"Tactot\",\"type\":\"Tactot\"},\"mediaFileDuration\":1,\"name\":\"UpperBothPing1\",\"tracks\":[{\"effects\":[{\"modes\":{\"VestBack\":{\"dotMode\":{\"dotConnected\":true,\"feedback\":[{\"endTime\":119,\"playbackType\":\"NONE\",\"pointList\":[{\"index\":4,\"intensity\":0.3},{\"index\":3,\"intensity\":0.3}],\"startTime\":0},{\"endTime\":239,\"playbackType\":\"NONE\",\"pointList\":[{\"index\":7,\"intensity\":0.3},{\"index\":0,\"intensity\":0.3}],\"startTime\":119},{\"endTime\":359,\"playbackType\":\"NONE\",\"pointList\":[{\"index\":4,\"intensity\":0.3},{\"index\":3,\"intensity\":0.3}],\"startTime\":239},{\"endTime\":359,\"playbackType\":\"NONE\",\"pointList\":[{\"index\":7,\"intensity\":0.3},{\"index\":0,\"intensity\":0.3}],\"startTime\":359}]},\"mode\":\"DOT_MODE\",\"pathMode\":{\"feedback\":[{\"movingPattern\":\"CONST_SPEED\",\"playbackType\":\"NONE\",\"visible\":true,\"pointList\":[]}]}},\"VestFront\":{\"dotMode\":{\"dotConnected\":true,\"feedback\":[{\"endTime\":119,\"playbackType\":\"NONE\",\"pointList\":[{\"index\":4,\"intensity\":0.6},{\"index\":3,\"intensity\":0.6}],\"startTime\":0},{\"endTime\":239,\"playbackType\":\"NONE\",\"pointList\":[{\"index\":7,\"intensity\":0.6},{\"index\":0,\"intensity\":0.6}],\"startTime\":119},{\"endTime\":359,\"playbackType\":\"NONE\",\"pointList\":[{\"index\":4,\"intensity\":0.6},{\"index\":3,\"intensity\":0.6}],\"startTime\":239},{\"endTime\":359,\"playbackType\":\"NONE\",\"pointList\":[{\"index\":7,\"intensity\":0.6},{\"index\":0,\"intensity\":0.6}],\"startTime\":359}]},\"mode\":\"DOT_MODE\",\"pathMode\":{\"feedback\":[{\"movingPattern\":\"CONST_SPEED\",\"playbackType\":\"NONE\",\"visible\":true,\"pointList\":[]}]}}},\"name\":\"Effect 1\",\"offsetTime\":359,\"startTime\":0}],\"enable\":true},{\"enable\":true,\"effects\":[]}],\"updatedAt\":1593681836617,\"id\":\"-MBDpmuaHkvAAgc9macg\"},\"durationMillis\":0,\"intervalMillis\":20,\"size\":20}";

        private HapticPlayer hapticPlayer;

        public BhapticsIntegration(IntPtr value) : base(value)
        {
        }

        public void Setup()
        {
            hapticPlayer = new HapticPlayer();
            hapticPlayer.RegisterTactFileStr(FIRE_KEY, EXPLOSION_HAPTIC_STR);

            PlayerFireWeaponEvents.OnPlayerFireWeapon += PlayWeaponFireHaptics;
        }

        private void OnDestroy()
        {
            PlayerFireWeaponEvents.OnPlayerFireWeapon -= PlayWeaponFireHaptics;
        }

        private void PlayWeaponFireHaptics(Weapon weapon)
        {
            hapticPlayer.SubmitRegistered(FIRE_KEY);
        }
    }
}