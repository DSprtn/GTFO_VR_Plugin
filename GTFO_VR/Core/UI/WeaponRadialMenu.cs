using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Events;
using Player;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace GTFO_VR.Core.UI
{
    public class WeaponRadialMenu : MonoBehaviour
    {


        public WeaponRadialMenu(IntPtr value)
: base(value) { }

        public static Dictionary<InputAction, bool> hackyInput = new Dictionary<InputAction, bool>();

        Dictionary<InventorySlot, RadialItem> m_radialItems;

        Dictionary<InventorySlot, Sprite> m_retrievedIcons;

        RadialMenu m_radialMenu;


        SteamVR_Action_Boolean m_weaponRadialAction;

        public void Setup(Transform parent)
        {
            
            m_radialItems = new Dictionary<InventorySlot, RadialItem>();
            m_retrievedIcons = new Dictionary<InventorySlot, Sprite>();

            hackyInput[InputAction.SelectMelee] = false;
            hackyInput[InputAction.SelectConsumable] = false;
            hackyInput[InputAction.SelectHackingTool] = false;
            hackyInput[InputAction.SelectResourcePack] = false;
            hackyInput[InputAction.SelectStandard] = false;
            hackyInput[InputAction.SelectSpecial] = false;
            hackyInput[InputAction.SelectTool] = false;

            m_weaponRadialAction = SteamVR_Input.GetBooleanAction("WeaponRadialMenu");

            m_radialMenu = new GameObject("WeaponRadial").AddComponent<RadialMenu>();
            m_radialMenu.Setup(VR_Input.InteractionHand.MainHand);
            m_radialMenu.transform.SetParent(parent);

            m_radialMenu.AddRadialItem("Melee", WantToSelectMelee, out RadialItem melee);
            m_radialMenu.AddRadialItem("Primary", WantToSelectPrimary, out RadialItem primary);
            m_radialMenu.AddRadialItem("Secondary", WantToSelectSecondary, out RadialItem secondary);
            m_radialMenu.AddRadialItem("Tool", WantToSelectTool, out RadialItem tool);
            m_radialMenu.AddRadialItem("Consumable", WantToSelectConsumable, out RadialItem consumable);
            m_radialMenu.AddRadialItem("Pack", WantToSelectPack, out RadialItem pack);
            m_radialMenu.AddRadialItem("HackingTool", WantToSelectHackingTool, out RadialItem hackingTool);

            melee.SetIcon(VRAssets.MeleeFallback);
            primary.SetIcon(VRAssets.PrimaryFallback);
            secondary.SetIcon(VRAssets.SecondaryFallback);
            tool.SetIcon(VRAssets.ToolFallback);
            consumable.SetIcon(VRAssets.ThrowableFallback);
            pack.SetIcon(VRAssets.PackFallback);
            hackingTool.SetIcon(VRAssets.HackingToolFallback);

            pack.Active = false;
            consumable.Active = false;

            m_radialItems.Add(InventorySlot.GearMelee, melee);
            m_radialItems.Add(InventorySlot.GearStandard, primary);
            m_radialItems.Add(InventorySlot.GearSpecial, secondary);
            m_radialItems.Add(InventorySlot.GearClass, tool);
            m_radialItems.Add(InventorySlot.Consumable, consumable);
            m_radialItems.Add(InventorySlot.ResourcePack, pack);
            m_radialItems.Add(InventorySlot.HackingTool, hackingTool);

            InventoryAmmoEvents.OnInventoryAmmoUpdate += AmmoUpdate;
            BackpackEvents.OnNewItemStatus += ItemStatusChanged;
        }

        private void ItemStatusChanged(InventorySlot slot, eInventoryItemStatus status)
        {
            if(m_radialItems.TryGetValue(slot, out RadialItem item))
            {
                if(status.Equals(eInventoryItemStatus.Deployed))
                {
                    item.Active = false;
                } else
                {
                    item.Active = true;
                }
            }
        }


        private void AmmoUpdate(InventorySlotAmmo item, int clipLeft)
        {
            if(item == null)
            {
                Log.Warning("Got null item!");
                return;
            }
            if(m_radialItems.TryGetValue(item.Slot, out RadialItem radialItem))
            {
                UpdateItemInfoText(item, clipLeft, radialItem);
                TryGetIcon(item, radialItem);
            }
        }

        private void TryGetIcon(InventorySlotAmmo item, RadialItem radialItem)
        {
            if(m_retrievedIcons == null)
            {
                Log.Error("Retrieved icon dict is null");
                return;
            }
            if(m_retrievedIcons.ContainsKey(item.Slot))
            {
                return;
            }

            if(PlayerBackpackManager.LocalBackpack.TryGetBackpackItem(item.Slot, out BackpackItem bp))
            {
                if(bp == null || bp.Instance == null)
                {
                    Log.Warning("Got null BP or instance");
                    return;
                }
                ItemEquippable equippable = bp.Instance.Cast<ItemEquippable>();
                if (equippable == null || equippable.GearIDRange == null)
                {
                    Log.Warning("Equippable was null or GearIDRange was null when retrieving icon!");
                    return;
                }
                if(GearIconRendering.TryGetGearIconSprite(equippable.GearIDRange.GetChecksum(), out Sprite icon))
                {
                    if(icon == null)
                    {
                        m_retrievedIcons[item.Slot] = null;
                        Log.Debug($"Failed to retrieve icon for {equippable.ArchetypeName}");
                        return;
                    }
                    m_retrievedIcons[item.Slot] = icon;
                    radialItem.SetIcon(icon);
                    Log.Debug($"Retrieved icon for {equippable.ArchetypeName}!");
                } else
                {
                    m_retrievedIcons[item.Slot] = null;
                    Log.Debug($"Failed to retrieve icon for {equippable.ArchetypeName}");
                }
            }
        }

        private static void UpdateItemInfoText(InventorySlotAmmo item, int clipLeft, RadialItem radialItem)
        {
            if(item == null)
            {
                return;
            }
            if (item.Slot == InventorySlot.GearStandard || item.Slot == InventorySlot.GearSpecial)
            {
                string ammoStatus = clipLeft + "\n----\n" + ((int)(item.BulletsMaxCap * item.RelInPack)).ToString();
                radialItem.SetInfoText(ammoStatus);
            }

            if (item.Slot == InventorySlot.GearClass)
            {
                radialItem.SetInfoText($"{(int) (item.RelInPack * 100f)}%");
            }

            if (item.Slot == InventorySlot.Consumable || item.Slot == InventorySlot.ResourcePack)
            {
                int CurrentValue = (int)(item.BulletsMaxCap * item.RelInPack) + clipLeft;
                radialItem.SetInfoText(CurrentValue.ToString());
                radialItem.Active = CurrentValue > 0;
            }
        }

        void Update()
        {
            if (m_weaponRadialAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                m_radialMenu.Show();
            }
            if (m_weaponRadialAction.GetStateUp(SteamVR_Input_Sources.Any))
            {
                m_radialMenu.Hide();
            }
        }

        void OnDestroy()
        {
            if(m_radialMenu)
            {
                Destroy(m_radialMenu);
            }
            InventoryAmmoEvents.OnInventoryAmmoUpdate -= AmmoUpdate;
            BackpackEvents.OnNewItemStatus -= ItemStatusChanged;
        }

        internal static bool GetSpecialActionMappingDown(InputAction action)
        {
            if (hackyInput.ContainsKey(action))
            {
                if(hackyInput[action])
                {
                    hackyInput[action] = false;
                    return true;
                }
            }
            return false;
        }

        public void WantToSelectMelee()
        {
            hackyInput[InputAction.SelectMelee] = true;
        }

        public void WantToSelectPrimary()
        {
            hackyInput[InputAction.SelectStandard] = true;
        }

        public void WantToSelectSecondary()
        {
            hackyInput[InputAction.SelectSpecial] = true;
        }

        public void WantToSelectTool()
        {
            hackyInput[InputAction.SelectTool] = true;
        }

        public void WantToSelectConsumable()
        {
            hackyInput[InputAction.SelectConsumable] = true;
        }

        public void WantToSelectPack()
        {
            hackyInput[InputAction.SelectResourcePack] = true;
        }

        public void WantToSelectHackingTool()
        {
            hackyInput[InputAction.SelectHackingTool] = true;
        }
    }
}
