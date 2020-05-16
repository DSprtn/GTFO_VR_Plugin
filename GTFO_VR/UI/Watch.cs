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
        Vector3 handOffset = new Vector3(0, -.05f, -.15f);
        Quaternion handRotationOffset = Quaternion.Euler(new Vector3(205, -100f, -180f));

        static DividedBarShaderController Health;
        static DividedBarShaderController Infection;
        DividedBarShaderController BulletsInMag;

        Dictionary<InventorySlot, DividedBarShaderController> UIMappings = new Dictionary<InventorySlot, DividedBarShaderController>();

        WatchState currentState = WatchState.Inventory;

        MeshRenderer[] inventoryMeshes;

        GameObject objectiveParent;

        static TextMeshPro objectiveDisplay;

        enum WatchState
        {
            Inventory, 
            Objective
        }

        void Awake()
        {
            Setup();
            SwitchState(WatchState.Inventory);
        }
        private void Setup()
        {
            ItemEquippableEvents.OnPlayerWieldItem += ItemSwitched;
            InventoryAmmoEvents.OnInventoryAmmoUpdate += UiUpdate;
            transform.SetParent(Controllers.offhandController.transform);
            transform.localPosition = handOffset;
            transform.localRotation = handRotationOffset;

            inventoryMeshes = transform.FindChildRecursive("Inventory_UI").GetComponentsInChildren<MeshRenderer>();
            objectiveParent = transform.FindChildRecursive("WardenObjective").gameObject;

            RectTransform watchObjectiveTransform = objectiveParent.GetComponent<RectTransform>();
            objectiveDisplay = objectiveParent.AddComponent<TextMeshPro>();

            objectiveDisplay.enableAutoSizing = true;
            objectiveDisplay.fontSizeMin = 18;
            objectiveDisplay.fontSizeMax = 36;

            StartCoroutine(SetRectSize(watchObjectiveTransform, new Vector2(42, 34f)));

            objectiveDisplay.color = DividedBarShaderController.normalColor * 1.35f;

            UIMappings.Add(InventorySlot.GearStandard, transform.FindChildRecursive("MainWeapon").gameObject.AddComponent<DividedBarShaderController>());
            UIMappings.Add(InventorySlot.GearSpecial, transform.FindChildRecursive("SubWeapon").gameObject.AddComponent<DividedBarShaderController>());
            UIMappings.Add(InventorySlot.GearClass, transform.FindChildRecursive("Tool").gameObject.AddComponent<DividedBarShaderController>());
            UIMappings.Add(InventorySlot.ResourcePack, transform.FindChildRecursive("Pack").gameObject.AddComponent<DividedBarShaderController>());
            UIMappings.Add(InventorySlot.Consumable, transform.FindChildRecursive("Consumable").gameObject.AddComponent<DividedBarShaderController>());
            UIMappings.Add(InventorySlot.ConsumableHeavy, UIMappings[InventorySlot.Consumable]);

            Health = transform.FindChildRecursive("HP").gameObject.AddComponent<DividedBarShaderController>();
            Infection = transform.FindChildRecursive("Infection").gameObject.AddComponent<DividedBarShaderController>();
            BulletsInMag = transform.FindChildRecursive("Ammo").gameObject.AddComponent<DividedBarShaderController>();

            Health.SetColor(new Color(0.33f, 0f, 0f));
            Infection.SetColor(new Color(0.533f / 3f, 1 / 3f, 0.8f / 3f));

            Health.maxAmmo = 100;
            Health.currentAmmo = 100;

            Infection.maxAmmo = 100;
            Infection.currentAmmo = 0;

            Health.UpdateShaderVals(5, 2);
            Infection.UpdateShaderVals(5, 2);
        }

        public void SwitchState()
        {
            if(currentState.Equals(WatchState.Inventory))
            {
                SwitchState(WatchState.Objective);
            } else
            {
                SwitchState(WatchState.Inventory);
            }
            VRInput.TriggerHapticPulse(0.025f, 1 / .025f, 0.3f, Controllers.GetDeviceFromType(Controllers.offHandControllerType));
        }

        void SwitchState(WatchState state)
        {
            switch(state) {
                case (WatchState.Inventory):
                    ToggleInventoryRendering(true);
                    ToggleObjectiveRendering(false);
                    currentState = WatchState.Inventory;
                    break;
                case (WatchState.Objective):
                    ToggleInventoryRendering(false);
                    ToggleObjectiveRendering(true);
                    currentState = WatchState.Objective;
                    break;
            }
        }

        public static void UpdateObjectives(string mainObj, string subObj)
        {
            if(objectiveDisplay != null)
            {
                objectiveDisplay.text = "WARDEN OBJECTIVE: \n \n " + mainObj + "\n \n " + subObj;
                VRInput.TriggerHapticPulse(0.01f, 1 / .025f, 0.2f, Controllers.GetDeviceFromType(Controllers.offHandControllerType));
            }
            
        }

        void ToggleInventoryRendering(bool toggle)
        {
            foreach (MeshRenderer m in inventoryMeshes)
            {
                m.enabled = toggle;
            }
        }

        void ToggleObjectiveRendering(bool toggle)
        {
            objectiveDisplay.enabled = toggle;
        }

        

        IEnumerator SetRectSize(RectTransform t, Vector2 size)
        {
            yield return new WaitForSeconds(0.1f);
            t.sizeDelta = size;
        }

        public static void UpdateInfection(float infection)
        {
            if(Infection)
            {
                Infection.SetFill(infection);
            }
        }

        public static void UpdateHealth(float health)
        {
            if (Health)
            {
                Health.SetFill(health);
            }
        }

        // Handle selection effect and grid updates
        private void ItemSwitched(ItemEquippable item)
        {
            DividedBarShaderController UIBar = null;
            foreach (DividedBarShaderController d in UIMappings.Values)
            {
                d.SetUnselected();
            }
            UIMappings.TryGetValue(item.ItemDataBlock.inventorySlot, out UIBar);

            if(UIBar)
            {
                UIBar.SetSelected();
            }

            if(ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                //Debug.Log("Item changed event: " + "MaxAmmo: " + item.GetMaxClip() + " Current ammo: " + item.GetCurrentClip() + " Inventory slot: " + item.ItemDataBlock.inventorySlot);
                BulletsInMag.maxAmmo = item.GetMaxClip();
                BulletsInMag.currentAmmo = item.GetCurrentClip();
                BulletsInMag.UpdateAmmoGridDivisions();
                BulletsInMag.inventorySlot = item.ItemDataBlock.inventorySlot;
            } else
            {
                BulletsInMag.currentAmmo = 0;
                BulletsInMag.UpdateShaderVals(1, 1);
            }

        }

        // Listen for values in respective classes ( PUI_Inventory )
        private void UiUpdate(InventorySlotAmmo item, int clipLeft)
        {
            DividedBarShaderController bar = null;
            UIMappings.TryGetValue(item.Slot, out bar);

            if (ItemEquippableEvents.currentItem != null && 
                ItemEquippableEvents.currentItem.ItemDataBlock.inventorySlot.Equals(item.Slot) && 
                ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                //Debug.Log("Ammo update event (main or sub weapon): " + "MaxAmmo: " + item.BulletClipSize + " Current ammo: " + clipLeft + " Inventory slot: " + item.Slot);

                BulletsInMag.maxAmmo = Mathf.Max(item.BulletClipSize, 1);
                BulletsInMag.UpdateCurrentAmmo(clipLeft);
                BulletsInMag.UpdateAmmoGridDivisions();
                BulletsInMag.inventorySlot = item.Slot;
            }

            
            if (bar)
            {
                //Debug.Log("Ammo update event: " + "MaxAmmo: " + item.BulletClipSize + " Current ammo: " + clipLeft + " maxCap: " + item.BulletsMaxCap + " RelInPack " + item.RelInPack + " Inventory slot: " + item.Slot);
                bar.maxAmmo = item.BulletsMaxCap;
                bar.currentAmmo = (int) (bar.maxAmmo * item.RelInPack);
                bar.SetFill(item.RelInPack);

                if (item.Slot.Equals(InventorySlot.Consumable) || item.Slot.Equals(InventorySlot.ResourcePack) || item.Slot.Equals(InventorySlot.ConsumableHeavy))
                {
                    bar.UpdatePackOrConsumableDivisions();
                }
            }
        }

        void OnDestroy()
        {
            ItemEquippableEvents.OnPlayerWieldItem -= ItemSwitched;
            InventoryAmmoEvents.OnInventoryAmmoUpdate -= UiUpdate;
            objectiveDisplay = null;
            Health = null;
            Infection = null;
        }

    }
}
