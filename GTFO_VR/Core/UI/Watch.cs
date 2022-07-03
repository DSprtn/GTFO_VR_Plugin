using GTFO_VR.Core;
using GTFO_VR.Core.PlayerBehaviours;
using GTFO_VR.Core.UI;
using GTFO_VR.Core.VR_Input;
using GTFO_VR.Events;
using GTFO_VR.Util;
using Player;
using SteamVR_Standalone_IL2CPP.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using Valve.VR;
using Mathf = SteamVR_Standalone_IL2CPP.Util.Mathf;

namespace GTFO_VR.UI
{
    /// <summary>
    /// Handles all VR watch UI related functions
    /// </summary>
    
    // ToDO - Refactor this into something more manageable, or not, if no new UI is planned.

    public class Watch : MonoBehaviour
    {

        internal enum WatchState
        {
            Inventory,
            Objective,
            Chat
        }

        public Watch(IntPtr value): base(value) { }

        public static Watch Current;


        RadialMenu m_watchRadialMenu;

        Dictionary<InventorySlot, DividedBarShaderController> m_inventoryToAmmoDisplayMapping = new Dictionary<InventorySlot, DividedBarShaderController>();
        DividedBarShaderController m_bulletsInMagDisplay;
        TextMeshPro m_numberBulletsInMagDisplay;

        DividedBarShaderController m_healthDisplay;
        DividedBarShaderController m_infectionDisplay;
        DividedBarShaderController m_oxygenDisplay;
        TextMeshPro m_objectiveDisplay;
        TextMeshPro m_chatDisplay;

        Queue<string> msgBuffer = new Queue<string>();

        readonly Color m_normalHealthCol = new Color(0.66f, 0f, 0f);
        readonly Color m_normalInfectionCol = new Color(0.533f, 1, 0.8f);
        readonly Color m_normalOxygenCol = Color.cyan;

        MeshRenderer[] m_inventoryMeshes;
        WatchState m_currentState = WatchState.Inventory;

        Vector3 m_handOffset = new Vector3(0, -.05f, -.15f);
        Quaternion m_leftHandRotationOffset = Quaternion.Euler(new Vector3(205, -100f, -180f));
        Quaternion m_rightHandRotationOffset = Quaternion.Euler(new Vector3(205, 100f, 180f));

        string m_ObjectiveText;
        Regex INDENT_REGEX = new Regex(@"<indent=\d{1,3}%>");

        SteamVR_Action_Boolean toggleWatchMode;
        SteamVR_Action_Boolean watchRadialMenu;

        void Awake()
        {
            watchRadialMenu = SteamVR_Input.GetBooleanAction("WatchRadialMenu");
            toggleWatchMode = SteamVR_Input.GetBooleanAction("ToggleWatchMode");
            Current = this;
            ItemEquippableEvents.OnPlayerWieldItem += ItemSwitched;
            InventoryAmmoEvents.OnInventoryAmmoUpdate += AmmoUpdate;
            Controllers.HandednessSwitched += SetHandedness;
            VRConfig.configWatchScaling.SettingChanged += WatchScaleChanged;
            VRConfig.configUseNumbersForAmmoDisplay.SettingChanged += AmmoDisplayChanged;
            VRConfig.configWatchColor.SettingChanged += WatchColorChanged;
            VRConfig.configWatchInfoText.SettingChanged += WatchRadialInfoTextChanged;
            ChatMsgEvents.OnChatMsgReceived += ChatMsgReceived;
        }

        private void WatchRadialInfoTextChanged(object sender, EventArgs e)
        {
            m_watchRadialMenu?.ToggleAllInfoText(VRConfig.configWatchInfoText.Value);
        }

        private void ChatMsgReceived(string msg)
        {
            if (msgBuffer.Contains(msg))
            {
                return;
            }
            SteamVR_InputHandler.TriggerHapticPulse(0.1f, 40f, .75f, Controllers.GetDeviceFromInteractionHandType(InteractionHand.Offhand));
            CellSound.Post(AK.EVENTS.GAME_MENU_CHANGE_PAGE, transform.position);
            msgBuffer.Enqueue(msg);
            if (msgBuffer.Count > 8) {
                msgBuffer.Dequeue();
            }
            m_chatDisplay.text = "";
            foreach(string chatMsg in msgBuffer)
            {
                m_chatDisplay.text += chatMsg + "\n";
            }
            m_chatDisplay.ForceMeshUpdate(false);
        }

        public void Setup(Transform parent)
        {
            m_inventoryMeshes = transform.FindDeepChild("Inventory_UI").GetComponentsInChildren<MeshRenderer>();

            SetupRadialMenu(parent);
            SetHandedness();
            SetupObjectiveDisplay();
            SetupChatDisplay();
            SetupInventoryLinkData();
            SetInitialPlayerStatusValues();
            SwitchState(m_currentState);
            SetWatchScale();
        }

        private void SetupChatDisplay()
        {
            GameObject chatParent = transform.FindDeepChild("Chat").gameObject;

            RectTransform chatTransform = chatParent.GetComponent<RectTransform>();
            m_chatDisplay = chatParent.AddComponent<TextMeshPro>();

            m_chatDisplay.enableAutoSizing = true;
            m_chatDisplay.fontSizeMin = 18;
            m_chatDisplay.fontSizeMax = 36;
            m_chatDisplay.alignment = TextAlignmentOptions.Center;
            MelonCoroutines.Start(SetRectSize(chatTransform, new Vector2(34, 43f)));
        }

        private void SetupRadialMenu(Transform parent)
        {
            m_watchRadialMenu = new GameObject("WatchRadial").AddComponent<RadialMenu>();
            m_watchRadialMenu.Setup(InteractionHand.Offhand, gameObject);
            m_watchRadialMenu.transform.SetParent(parent);
            m_watchRadialMenu.AddRadialItem("Inventory", SwitchToInventory, out RadialItem inventory);
            inventory.SetIcon(VRAssets.PrimaryFallback);
            inventory.SetInfoText("Inventory");

            m_watchRadialMenu.AddRadialItem("Objective", SwitchToObjective, out RadialItem objective);
            objective.SetIcon(VRAssets.Objective);
            objective.SetInfoText("Objective");

            m_watchRadialMenu.AddRadialItem("ChatType", TypeInChat, out RadialItem chatType);
            chatType.SetIcon(VRAssets.ChatType);
            chatType.SetInfoText("Type In Chat");

            m_watchRadialMenu.AddRadialItem("Chat", SwitchToChat, out RadialItem chat);
            chat.SetIcon(VRAssets.Chat);
            chat.SetInfoText("Chat");
        }

        public void TypeInChat()
        {
            if (PlayerChatManager.Current != null && !PlayerChatManager.InChatMode)
            {
                PlayerChatManager.Current.EnterChatMode();
            }
        }

        public void SwitchToChat()
        {
            SwitchState(WatchState.Chat);
        }

        public void SwitchToInventory()
        {
            SwitchState(WatchState.Inventory);
        }

        public void SwitchToObjective()
        {
            SwitchState(WatchState.Objective);
        }

        private void WatchColorChanged(object sender, EventArgs e)
        {
            SetWatchColor();
        }

        private void AmmoDisplayChanged(object sender, EventArgs e)
        {
            SwitchState(m_currentState);
        }

        void Start()
        {
            SetWatchColor();
        }

        private void SetWatchColor()
        {
            transform.FindDeepChild("STRAP_LP").GetComponent<MeshRenderer>().material.color = ExtensionMethods.FromString(VRConfig.configWatchColor.Value);
            transform.FindDeepChild("WATCH_LP").GetComponent<MeshRenderer>().material.color = ExtensionMethods.FromString(VRConfig.configWatchColor.Value);
        }

        void Update()
        {
            UpdateInput();
        }

        private void UpdateInput()
        {
            if (watchRadialMenu.GetStateDown(SteamVR_Input_Sources.Any))
            {
                m_watchRadialMenu.Show();
            }
            if (watchRadialMenu.GetStateUp(SteamVR_Input_Sources.Any))
            {
                m_watchRadialMenu.Hide();
            }
            if (toggleWatchMode.GetStateDown(SteamVR_Input_Sources.Any))
            {
                SwitchState();
            }
        }

        
        public void UpdateObjective(PUI_GameObjectives gameObjectives)
        {
            StringBuilder builder = new StringBuilder();

            // Main object is added to progressions too, as its subobjective. 
            foreach ( PUI_ProgressionObjective progression in gameObjectives.m_progressionObjectives)
            {
                if (progression == null)
                    continue;

                string header = progression.m_header?.text;
                if ( header != null)
                {
                    header = INDENT_REGEX.Replace(header, "");  // Indent breaks formatting
                    builder.Append(header);
                    builder.Append("\n");
                }

                string txt = progression.m_text?.text;
                if ( txt != null)
                {
                    txt = INDENT_REGEX.Replace(txt, "");
                    builder.Append(txt);
                }
            }
            this.m_ObjectiveText = builder.ToString();
            UpdateObjectiveDisplay();
        }
        public void UpdateObjectiveDisplay() {
            if (m_objectiveDisplay != null)
            {
                m_objectiveDisplay.text = m_ObjectiveText;
                m_objectiveDisplay.ForceMeshUpdate(false);
                SteamVR_InputHandler.TriggerHapticPulse(0.01f, 1 / .025f, 0.2f, Controllers.GetDeviceFromHandType(Controllers.offHandControllerType));
            }
        }

        public void UpdateInfection(float infection)
        {
            if (m_infectionDisplay)
            {
                if (infection < 0.01f)
                {
                    m_infectionDisplay.ToggleRendering(false);
                } else if(m_currentState == WatchState.Inventory)
                {
                    m_infectionDisplay.ToggleRendering(true);
                    m_infectionDisplay.UpdateFill((int) (infection * 100f));
                    m_infectionDisplay.SetFill(infection);
                    m_infectionDisplay.SetColor(Color.Lerp(m_normalInfectionCol, m_normalInfectionCol * 1.6f, infection));
                }
            }
        }

        public void UpdateHealth(float health)
        {
            if (m_healthDisplay)
            {
                m_healthDisplay.UpdateFill((int)(health * 100f));
                m_healthDisplay.SetColor(Color.Lerp(m_normalHealthCol, m_normalHealthCol * 1.8f, 1 - health));
            }
        }

        public void UpdateAir(float val)
        {
            if (m_oxygenDisplay)
            {
                if (val < .95f && m_currentState == WatchState.Inventory)
                {
                    m_oxygenDisplay.SetFill(val);
                    m_oxygenDisplay.UpdateFill((int)(val * 100f));
                    m_oxygenDisplay.ToggleRendering(true);

                    if (val < 0.5)
                    {
                        m_oxygenDisplay.SetColor(Color.Lerp(Color.red, m_normalOxygenCol, val * 1.6f));
                    }
                    else
                    {
                        m_oxygenDisplay.SetColor(Color.cyan);
                    }
                } else
                {
                    m_oxygenDisplay.ToggleRendering(false);
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
            UpdateBulletDisplayAmount(item, clipLeft);
            UpdateInventoryAmmoGrids(item, clipLeft);
        }

        private void HandleSelectionEffect(ItemEquippable item)
        {
            foreach (DividedBarShaderController d in m_inventoryToAmmoDisplayMapping.Values)
            {
                d.SetUnselected();
            }
            m_inventoryToAmmoDisplayMapping.TryGetValue(item.ItemDataBlock.inventorySlot, out DividedBarShaderController UIBar);

            if (UIBar)
            {
                UIBar.SetSelected();
            }
        }

        private void UpdateInventoryAmmoGrids(InventorySlotAmmo item, int clipLeft)
        {
            m_inventoryToAmmoDisplayMapping.TryGetValue(item.Slot, out DividedBarShaderController bar);
            if (bar)
            {
                bar.MaxValue = item.BulletsMaxCap;
                bar.CurrentValue = (int)(bar.MaxValue * item.RelInPack) + clipLeft;
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

        private void UpdateBulletDisplayAmount(InventorySlotAmmo item, int clipLeft)
        {
            if (ItemEquippableEvents.IsCurrentItemShootableWeapon() &&
                ItemEquippableEvents.currentItem.ItemDataBlock.inventorySlot.Equals(item.Slot))
            {
                m_numberBulletsInMagDisplay.text = clipLeft + "\n----\n" + ((int)(item.BulletsMaxCap * item.RelInPack)).ToString();
                m_numberBulletsInMagDisplay.ForceMeshUpdate(false);

                m_bulletsInMagDisplay.MaxValue = Mathf.Max(item.BulletClipSize, 1);
                m_bulletsInMagDisplay.UpdateCurrentAmmo(clipLeft);
                m_bulletsInMagDisplay.UpdateAmmoGridDivisions();
            }
        }

        private void UpdateBulletGridDivisions(ItemEquippable item)
        {

            if (ItemEquippableEvents.IsCurrentItemShootableWeapon())
            {
                if (!VRConfig.configUseNumbersForAmmoDisplay.Value)
                {
                    m_bulletsInMagDisplay.MaxValue = item.GetMaxClip();
                    m_bulletsInMagDisplay.CurrentValue = item.GetCurrentClip();
                    m_bulletsInMagDisplay.UpdateAmmoGridDivisions();
                }
            }
            else
            {
                m_bulletsInMagDisplay.CurrentValue = 0;
                m_bulletsInMagDisplay.UpdateShaderVals(1, 1);

                m_numberBulletsInMagDisplay.text = "";
                m_numberBulletsInMagDisplay.ForceMeshUpdate(false);
            }
        }

        private void SetHandedness()
        {
            transform.SetParent(Controllers.offhandController.transform);
            transform.localPosition = m_handOffset;
            if (!VRConfig.configUseLeftHand.Value)
            {
                transform.localRotation = m_leftHandRotationOffset;
            }
            else
            {
                transform.localRotation = m_rightHandRotationOffset;
            }
        }

        private void SetupObjectiveDisplay()
        {
            GameObject objectiveParent = transform.FindDeepChild("WardenObjective").gameObject;

            RectTransform watchObjectiveTransform = objectiveParent.GetComponent<RectTransform>();
            m_objectiveDisplay = objectiveParent.AddComponent<TextMeshPro>();

            m_objectiveDisplay.enableAutoSizing = true;
            m_objectiveDisplay.fontSizeMin = 18;
            m_objectiveDisplay.fontSizeMax = 36;
            m_objectiveDisplay.alignment = TextAlignmentOptions.Center;
            m_objectiveDisplay.faceColor = new Color32(255, 255, 255, 25); // Adjust alpha so all the text isn't just pure white
            MelonCoroutines.Start(SetRectSize(watchObjectiveTransform, new Vector2(34, 43f)));
        }

        IEnumerator SetRectSize(RectTransform t, Vector2 size)
        {
            yield return new WaitForEndOfFrame();
            t.sizeDelta = size;
        }

        private void SetupInventoryLinkData()
        {
            m_inventoryToAmmoDisplayMapping.Add(InventorySlot.GearStandard, transform.FindDeepChild("MainWeapon").gameObject.AddComponent<DividedBarShaderController>());
            m_inventoryToAmmoDisplayMapping.Add(InventorySlot.GearSpecial, transform.FindDeepChild("SubWeapon").gameObject.AddComponent<DividedBarShaderController>());
            m_inventoryToAmmoDisplayMapping.Add(InventorySlot.GearClass, transform.FindDeepChild("Tool").gameObject.AddComponent<DividedBarShaderController>());
            m_inventoryToAmmoDisplayMapping.Add(InventorySlot.ResourcePack, transform.FindDeepChild("Pack").gameObject.AddComponent<DividedBarShaderController>());
            m_inventoryToAmmoDisplayMapping.Add(InventorySlot.Consumable, transform.FindDeepChild("Consumable").gameObject.AddComponent<DividedBarShaderController>());
            m_inventoryToAmmoDisplayMapping.Add(InventorySlot.ConsumableHeavy, m_inventoryToAmmoDisplayMapping[InventorySlot.Consumable]);

            m_healthDisplay = transform.FindDeepChild("HP").gameObject.AddComponent<DividedBarShaderController>();
            m_oxygenDisplay = transform.FindDeepChild("Air").gameObject.AddComponent<DividedBarShaderController>();
            m_infectionDisplay = transform.FindDeepChild("Infection").gameObject.AddComponent<DividedBarShaderController>();

            m_numberBulletsInMagDisplay = transform.FindDeepChild("NumberedAmmo").gameObject.AddComponent<TextMeshPro>();

            m_numberBulletsInMagDisplay.lineSpacing = -30f;

            m_numberBulletsInMagDisplay.alignment = TextAlignmentOptions.Center;
            m_numberBulletsInMagDisplay.fontSize = 80f;
            m_numberBulletsInMagDisplay.enableWordWrapping = false;
            m_numberBulletsInMagDisplay.fontStyle = FontStyles.Bold;
            m_numberBulletsInMagDisplay.richText = true;
            m_numberBulletsInMagDisplay.color = DividedBarShaderController.NormalColor;
            m_bulletsInMagDisplay = transform.FindDeepChild("Ammo").gameObject.AddComponent<DividedBarShaderController>();
        }

        private void SetInitialPlayerStatusValues()
        {
            m_healthDisplay.SetColor(m_normalHealthCol);
            m_infectionDisplay.SetColor(m_normalInfectionCol);
            m_oxygenDisplay.SetColor(m_normalOxygenCol);

            m_healthDisplay.MaxValue = 100;
            m_healthDisplay.CurrentValue = 100;

            m_oxygenDisplay.MaxValue = 100;
            m_oxygenDisplay.CurrentValue = 100;

            m_infectionDisplay.MaxValue = 100;
            m_infectionDisplay.CurrentValue = 0;

            m_healthDisplay.UpdateShaderVals(5, 2);
            m_infectionDisplay.UpdateShaderVals(5, 2);
            m_oxygenDisplay.UpdateShaderVals(5, 2);

            UpdateAir(100f);
        }
        public void SwitchState()
        {
            int maxStateIndex = Enum.GetValues(typeof(WatchState)).Length - 1;
            int nextIndex = (int)m_currentState + 1;

            if (nextIndex > maxStateIndex)
            {
                nextIndex = 0;
            }
            SwitchState((WatchState)nextIndex);
            SteamVR_InputHandler.TriggerHapticPulse(0.025f, 1 / .025f, 0.3f, Controllers.GetDeviceFromHandType(Controllers.offHandControllerType));
        }

        void SwitchState(WatchState state)
        {
            m_currentState = state;
            switch (state)
            {
                case (WatchState.Inventory):
                    ToggleInventoryRendering(true);
                    ToggleObjectiveRendering(false);
                    ToggleChatRendering(false);
                    break;
                case (WatchState.Objective):
                    ToggleInventoryRendering(false);
                    ToggleObjectiveRendering(true);
                    ToggleChatRendering(false);
                    break;
                case (WatchState.Chat):
                    ToggleInventoryRendering(false);
                    ToggleObjectiveRendering(false);
                    ToggleChatRendering(true);
                    break;
            }
        }

        private void ToggleChatRendering(bool toggle)
        {
            m_chatDisplay.enabled = toggle;
            m_chatDisplay.ForceMeshUpdate(false);
        }

        void ToggleInventoryRendering(bool toggle)
        {
            foreach (MeshRenderer m in m_inventoryMeshes)
            {
                m.enabled = toggle;
            }

            if (VRConfig.configUseNumbersForAmmoDisplay.Value)
            {
                m_numberBulletsInMagDisplay.gameObject.SetActive(toggle);
                m_bulletsInMagDisplay.gameObject.SetActive(false);
                
            } else
            {
                m_numberBulletsInMagDisplay.gameObject.SetActive(false);
                m_bulletsInMagDisplay.gameObject.SetActive(toggle);
            }
            m_numberBulletsInMagDisplay.ForceMeshUpdate();
            //Force update to possibly disable/enable those bars depending on oxygen level/infection level
            UpdateAir(m_oxygenDisplay.CurrentValue / 100f);
            UpdateInfection(m_infectionDisplay.CurrentValue / 100f);
        }

        void ToggleObjectiveRendering(bool toggle)
        {
            m_objectiveDisplay.enabled = toggle;
            m_objectiveDisplay.ForceMeshUpdate();
        }

        private void WatchScaleChanged(object sender, EventArgs e)
        {
            SetWatchScale();
        }

        void SetWatchScale()
        {
            Vector3 watchScale = new Vector3(1.25f, 1.25f, 1.25f);
            watchScale *= VRConfig.configWatchScaling.Value;
            transform.localScale = watchScale;
        }

        void OnDestroy()
        {
            if(m_watchRadialMenu)
            {
                Destroy(m_watchRadialMenu);
            }
            ItemEquippableEvents.OnPlayerWieldItem -= ItemSwitched;
            InventoryAmmoEvents.OnInventoryAmmoUpdate -= AmmoUpdate;
            Controllers.HandednessSwitched -= SetHandedness;
            VRConfig.configUseNumbersForAmmoDisplay.SettingChanged -= AmmoDisplayChanged;
            VRConfig.configWatchScaling.SettingChanged -= WatchScaleChanged;
            VRConfig.configWatchColor.SettingChanged -= WatchColorChanged;
            VRConfig.configWatchInfoText.SettingChanged -= WatchRadialInfoTextChanged;
            ChatMsgEvents.OnChatMsgReceived -= ChatMsgReceived;
        }
    }
}
