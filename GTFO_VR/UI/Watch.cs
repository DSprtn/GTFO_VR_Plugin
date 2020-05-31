using GTFO_VR.Events;
using GTFO_VR.Input;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Valve.VR;


namespace GTFO_VR.UI
{
    public class Watch : MonoBehaviour
    {
        enum WatchState
        {
            Inventory,
            Objective
        }

        MeshRenderer[] inventoryMeshes;
        WatchState currentState = WatchState.Inventory;

        Vector3 handOffset = new Vector3(0, -.05f, -.15f);
        Quaternion handRotationOffset = Quaternion.Euler(new Vector3(205, -100f, -180f));

        static TextMeshPro objectiveDisplay;
        Dictionary<InventorySlot, DividedBarShaderController> UIMappings = new Dictionary<InventorySlot, DividedBarShaderController>();

        DividedBarShaderController BulletsInMag;
        static DividedBarShaderController Health;
        static DividedBarShaderController Infection;
        static DividedBarShaderController Oxygen;

        static readonly Color normalHealthCol = new Color(0.33f, 0f, 0f);
        static readonly Color normalInfectionCol = new Color(0.533f / 3f, 1 / 3f, 0.8f / 3f);
        static readonly Color normalOxygenCol = Color.cyan / 3f;

        void Awake()
        {
            ItemEquippableEvents.OnPlayerWieldItem += ItemSwitched;
            InventoryAmmoEvents.OnInventoryAmmoUpdate += AmmoUpdate;

            Setup();
        }

        public static void UpdateObjectives(string mainObj, string subObj)
        {
            if(objectiveDisplay != null)
            {
                objectiveDisplay.text = "WARDEN OBJECTIVE: \n \n " + mainObj + "\n \n " + subObj;
                VRInput.TriggerHapticPulse(0.01f, 1 / .025f, 0.2f, Controllers.GetDeviceFromType(Controllers.offHandControllerType));
            }
        }

        public static void UpdateInfection(float infection)
        {
            if(Infection)
            {
                if(infection < 0.01f)
                {
                    Infection.ToggleRendering(false);
                } else
                {
                    Infection.SetFill(infection);
                    Infection.ToggleRendering(true);

                    Infection.SetColor(Color.Lerp(normalInfectionCol, normalInfectionCol * 1.6f, infection));
                }
            }
        }

        public static void UpdateHealth(float health)
        {
            if (Health)
            {
                Health.SetFill(health);
                Health.SetColor(Color.Lerp(normalHealthCol, normalHealthCol * 1.8f, 1 - health));
            }
        }

        public static void UpdateAir(float val)
        {
            if(Oxygen)
            {
                if(val < .95f)
                {
                    Oxygen.SetFill(val);
                    Oxygen.ToggleRendering(true);

                    if (val < 0.5)
                    {
                        Oxygen.SetColor(Color.Lerp(Color.red / 2f, normalOxygenCol, val * 1.6f));
                    }
                    else
                    {
                        Oxygen.SetColor(Color.cyan / 3f);
                    }
                } else
                {
                    Oxygen.ToggleRendering(false);
                }
            }
        }
        private void ItemSwitched(ItemEquippable item)
        {
            HandleSelectionEffect(item);
            UpdateBulletGridDivisions(item);
        }

        private void AmmoUpdate(InventorySlotAmmo item, int clipLeft)
        {
            UpdateBulletGridAmount(item, clipLeft);
            UpdateInventoryAmmoGrids(item, clipLeft);
        }

        private void HandleSelectionEffect(ItemEquippable item)
        {
            foreach (DividedBarShaderController d in UIMappings.Values)
            {
                d.SetUnselected();
            }
            UIMappings.TryGetValue(item.ItemDataBlock.inventorySlot, out DividedBarShaderController UIBar);

            if (UIBar)
            {
                UIBar.SetSelected();
            }
        }

        private void UpdateInventoryAmmoGrids(InventorySlotAmmo item, int clipLeft)
        {
            UIMappings.TryGetValue(item.Slot, out DividedBarShaderController bar);
            if (bar)
            {
                bar.maxValue = item.BulletsMaxCap;
                bar.currentValue = (int)(bar.maxValue * item.RelInPack) + clipLeft;
                bar.SetFill(item.RelInPack);

                if (item.Slot.Equals(InventorySlot.GearStandard) || item.Slot.Equals(InventorySlot.GearSpecial))
                {
                    bar.UpdateWeaponMagDivisions(item.BulletClipSize, item.BulletsMaxCap);
                }

                if (item.Slot.Equals(InventorySlot.Consumable) || item.Slot.Equals(InventorySlot.ResourcePack) || item.Slot.Equals(InventorySlot.ConsumableHeavy))
                {
                    bar.UpdatePackOrConsumableDivisions();
                }
            }
        }

        private void UpdateBulletGridAmount(InventorySlotAmmo item, int clipLeft)
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon() &&
                ItemEquippableEvents.currentItem.ItemDataBlock.inventorySlot.Equals(item.Slot))
            {
                BulletsInMag.maxValue = Mathf.Max(item.BulletClipSize, 1);
                BulletsInMag.UpdateCurrentAmmo(clipLeft);
                BulletsInMag.UpdateAmmoGridDivisions();
                BulletsInMag.inventorySlot = item.Slot;
            }
        }

        private void UpdateBulletGridDivisions(ItemEquippable item)
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                BulletsInMag.maxValue = item.GetMaxClip();
                BulletsInMag.currentValue = item.GetCurrentClip();
                BulletsInMag.UpdateAmmoGridDivisions();
                BulletsInMag.inventorySlot = item.ItemDataBlock.inventorySlot;
            }
            else
            {
                BulletsInMag.currentValue = 0;
                BulletsInMag.UpdateShaderVals(1, 1);
            }
        }

        private void Setup()
        {
            inventoryMeshes = transform.FindChildRecursive("Inventory_UI").GetComponentsInChildren<MeshRenderer>();

            SetupTransform();
            SetupObjectiveDisplay();
            SetupInventoryLinkData();
            SetInitialPlayerStatusValues();
            SwitchState(currentState);
        }

        private void SetupTransform()
        {
            transform.SetParent(Controllers.offhandController.transform);
            transform.localPosition = handOffset;
            transform.localRotation = handRotationOffset;
        }

        private void SetupObjectiveDisplay()
        {
            GameObject objectiveParent = transform.FindChildRecursive("WardenObjective").gameObject;

            RectTransform watchObjectiveTransform = objectiveParent.GetComponent<RectTransform>();
            objectiveDisplay = objectiveParent.AddComponent<TextMeshPro>();

            objectiveDisplay.enableAutoSizing = true;
            objectiveDisplay.fontSizeMin = 18;
            objectiveDisplay.fontSizeMax = 36;

            StartCoroutine(SetRectSize(watchObjectiveTransform, new Vector2(42, 34f)));
        }

        IEnumerator SetRectSize(RectTransform t, Vector2 size)
        {
            yield return new WaitForSeconds(0.1f);
            t.sizeDelta = size;
        }

        private void SetupInventoryLinkData()
        {
            UIMappings.Add(InventorySlot.GearStandard, transform.FindChildRecursive("MainWeapon").gameObject.AddComponent<DividedBarShaderController>());
            UIMappings.Add(InventorySlot.GearSpecial, transform.FindChildRecursive("SubWeapon").gameObject.AddComponent<DividedBarShaderController>());
            UIMappings.Add(InventorySlot.GearClass, transform.FindChildRecursive("Tool").gameObject.AddComponent<DividedBarShaderController>());
            UIMappings.Add(InventorySlot.ResourcePack, transform.FindChildRecursive("Pack").gameObject.AddComponent<DividedBarShaderController>());
            UIMappings.Add(InventorySlot.Consumable, transform.FindChildRecursive("Consumable").gameObject.AddComponent<DividedBarShaderController>());
            UIMappings.Add(InventorySlot.ConsumableHeavy, UIMappings[InventorySlot.Consumable]);

            Health = transform.FindChildRecursive("HP").gameObject.AddComponent<DividedBarShaderController>();
            Oxygen = transform.FindChildRecursive("Air").gameObject.AddComponent<DividedBarShaderController>();
            Infection = transform.FindChildRecursive("Infection").gameObject.AddComponent<DividedBarShaderController>();
            BulletsInMag = transform.FindChildRecursive("Ammo").gameObject.AddComponent<DividedBarShaderController>();
        }

        private static void SetInitialPlayerStatusValues()
        {
            Health.SetColor(normalHealthCol);
            Infection.SetColor(normalInfectionCol);
            Oxygen.SetColor(normalOxygenCol);

            Health.maxValue = 100;
            Health.currentValue = 100;

            Oxygen.maxValue = 100;
            Oxygen.currentValue = 100;

            Infection.maxValue = 100;
            Infection.currentValue = 0;

            Health.UpdateShaderVals(5, 2);
            Infection.UpdateShaderVals(5, 2);
            Oxygen.UpdateShaderVals(5, 2);

            UpdateAir(1f);
        }
        public void SwitchState()
        {
            int maxStateIndex = Enum.GetValues(typeof(WatchState)).Length - 1;
            int nextIndex = (int)currentState + 1;

            if (nextIndex > maxStateIndex)
            {
                nextIndex = 0;
            }
            SwitchState((WatchState)nextIndex);
            VRInput.TriggerHapticPulse(0.025f, 1 / .025f, 0.3f, Controllers.GetDeviceFromType(Controllers.offHandControllerType));
        }

        void SwitchState(WatchState state)
        {
            switch (state)
            {
                case (WatchState.Inventory):
                    ToggleInventoryRendering(true);
                    ToggleObjectiveRendering(false);

                    break;
                case (WatchState.Objective):
                    ToggleInventoryRendering(false);
                    ToggleObjectiveRendering(true);
                    break;
            }
            currentState = state;
        }
        void ToggleInventoryRendering(bool toggle)
        {
            foreach (MeshRenderer m in inventoryMeshes)
            {
                m.enabled = toggle;
            }
            //Force update to possibly disable those bars depending on oxygen level/infection level
            UpdateAir(Oxygen.currentValue);
            UpdateInfection(Infection.currentValue);
        }

        void ToggleObjectiveRendering(bool toggle)
        {
            objectiveDisplay.enabled = toggle;
        }

        void OnDestroy()
        {
            ItemEquippableEvents.OnPlayerWieldItem -= ItemSwitched;
            InventoryAmmoEvents.OnInventoryAmmoUpdate -= AmmoUpdate;
            objectiveDisplay = null;
            Health = null;
            Infection = null;
            Oxygen = null;
        }
    }
}
