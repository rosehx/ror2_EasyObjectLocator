using BepInEx.Configuration;
using EasyObjectLocator.Abstract;
using EasyObjectLocator.Network.Messages;
using EasyObjectLocator.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.UI;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EasyObjectLocator.Locators.Teleporter
{
    internal sealed class TeleporterLocator : Locator
    {
        private ConfigEntry<bool> _showTeleporterAfterStateChange;
        private ConfigEntry<int> _showTeleporterAfterXSeconds;
        private ConfigEntry<bool> _showTeleporterAlways;

        private Guid _teleporterLocatorId = Guid.Empty;
        private GameObject _teleporterLocatorObject;
        public TeleporterLocator(IContext core) : base(core)
        {
        }

        public override string ComponentId => "rosehx.EasyObjectLocator.Locators.TeleporterLocator";

        public override void DestroyObjects()
        {
            if (NetworkHelper.IsClient() || _teleporterLocatorObject == null)
                return;

            if (NetworkHelper.IsServer())
            {
                Factory.Logger.LogDebug($"Locator DestroyObjects - Server: \"{GetType().Name}\"");

                SyncLocatorMessage.WithData(
                    ComponentId,
                    _teleporterLocatorId,
                    _teleporterLocatorObject.transform.position,
                    (int)TeleporterInstanceType.PositionIndicator,
                    (int)TeleporterSyncType.Destroy
                ).Send(NetworkDestination.Clients);
            }

            DestroyLocator();
        }

        public override void ExtendConfig()
        {
            ConfigFile config = Factory.Config;

            _showTeleporterAlways = config.Bind(
                new ConfigDefinition("Teleporter", $"{ComponentId}.ShowAlways"),
                false,
                new ConfigDescription("Always show the teleporter location.")
            );

            _showTeleporterAfterXSeconds = config.Bind(
                new ConfigDefinition("Teleporter", $"{ComponentId}.ShowAfterXSeconds"),
                60,
                new ConfigDescription("Show teleporter after a certain amount of seconds. 0 seconds wont set a timer. If \"ShowAlways\" is also false the teleporter is never visible.")
            );

            _showTeleporterAfterStateChange = config.Bind(
                new ConfigDefinition("Teleporter", $"{ComponentId}.ShowAfterStateChange"),
                false,
                new ConfigDescription("Normally if you interact with the teleporter a charging Icon shows up and the locater is removed. If you enabled this the locater stays visible.")
            );
        }

        public override void ExtendHooks()
        {
            if (NetworkHelper.IsClient())
                return;
            On.RoR2.TeleporterInteraction.OnInteractionBegin += TeleporterInteraction_OnInteractionBegin;
        }

        public override void HandleIncomingMessage(Guid instanceId, Vector3 position, int instanceType, int syncType)
        {
            if (!NetworkHelper.IsClient())
            {
                Factory.Logger.LogWarning($"Locator HandleIncomingMessage - Invalid: \"{GetType().Name} (client={NetworkHelper.IsClient()},positionIndicator={_teleporterLocatorObject == null})\"");
                return;
            }

            if (!TryParseEnum<TeleporterInstanceType>(instanceType, out TeleporterInstanceType instanceTypeResult))
                return;

            if (!TryParseEnum<TeleporterSyncType>(syncType, out TeleporterSyncType syncTypeResult))
                return;

            Factory.Logger.LogDebug($"Locator HandleIncomingMessage - Pre: \"{GetType().Name} (id={instanceId},type={instanceTypeResult},sync={syncTypeResult})\" ");

            if (instanceTypeResult == TeleporterInstanceType.PositionIndicator)
            {
                if (syncTypeResult == TeleporterSyncType.Enable)
                {
                    if (_teleporterLocatorObject == null)
                    {
                        CreatePositionLocator(instanceId, position);
                    }
                }

                ToggleIndicator(syncTypeResult == TeleporterSyncType.Enable);

                if (syncTypeResult == TeleporterSyncType.Destroy)
                    DestroyLocator();
            }
        }

        public override void Initialize()
        {
            if (NetworkHelper.IsClient())
                return;

            if (_showTeleporterAlways.Value)
            {
                Factory.Logger.LogDebug($"Locator Initialize - ShowAlways: \"{GetType().Name}\"");
                Enable();
            }
            else if (!_showTeleporterAlways.Value && _showTeleporterAfterXSeconds.Value > 0)
            {
                Factory.Logger.LogDebug($"Locator Initialize - ShowDelays: \"{GetType().Name}\" (delay={_showTeleporterAfterXSeconds.Value})");
                Context.DelayedCall(Enable, _showTeleporterAfterXSeconds.Value);
            }
        }

        public override void RemoveHooks()
        {
            if (NetworkHelper.IsClient())
                return;
            On.RoR2.TeleporterInteraction.OnInteractionBegin -= TeleporterInteraction_OnInteractionBegin;
        }

        protected override void Disable()
        {
            if (NetworkHelper.IsClient() || _teleporterLocatorObject == null)
            {
                Factory.Logger.LogWarning($"Locator Disable - NullOrClient: \"{GetType().Name} (client={NetworkHelper.IsClient()},positionIndicator={_teleporterLocatorObject == null})\"");
                return;
            }

            Factory.Logger.LogDebug($"Locator Disable: \"{GetType().Name}\"");

            ToggleIndicator(false);

            if (NetworkHelper.IsServer())
            {
                Factory.Logger.LogDebug($"Locator sent Disable message: \"{GetType().Name}\"");

                SyncLocatorMessage.WithData(
                    ComponentId,
                    _teleporterLocatorId,
                    _teleporterLocatorObject.transform.position,
                    (int)TeleporterInstanceType.PositionIndicator,
                    (int)TeleporterSyncType.Disable
                ).Send(NetworkDestination.Clients);
            }
        }

        protected override void Enable()
        {
            CreatePositionLocatorLocally();

            if (NetworkHelper.IsClient() || _teleporterLocatorObject == null)
            {
                Factory.Logger.LogWarning($"Locator Enable - NullOrClient: \"{GetType().Name} (client={NetworkHelper.IsClient()},positionIndicator={_teleporterLocatorObject == null})\"");
                return;
            }

            Factory.Logger.LogDebug($"Locator Enable: \"{GetType().Name}\"");

            ToggleIndicator(true);

            if (NetworkHelper.IsServer())
            {
                Factory.Logger.LogDebug($"Locator sent Enable message: \"{GetType().Name}\"");

                SyncLocatorMessage.WithData(
                    ComponentId,
                    _teleporterLocatorId,
                    _teleporterLocatorObject.transform.position,
                    (int)TeleporterInstanceType.PositionIndicator,
                    (int)TeleporterSyncType.Enable
                ).Send(NetworkDestination.Clients);
            }
        }

        private void CreatePositionLocator(Guid instanceId, Vector3 position)
        {
            Factory.Logger.LogDebug($"Locator Initialize - InteractionInit: \"{GetType().Name}\"");

            _teleporterLocatorId = instanceId;

            GameObject chargingIndicatorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Teleporters/TeleporterChargingPositionIndicator.prefab").WaitForCompletion();
            _teleporterLocatorObject = UnityEngine.Object.Instantiate(chargingIndicatorPrefab, position, Quaternion.identity);
            _teleporterLocatorObject.SetActive(false);

            PositionIndicator positionIndicator = _teleporterLocatorObject.GetComponent<PositionIndicator>();
            positionIndicator.gameObject.SetActive(false);

            ChargeIndicatorController chargeIndicatorController = positionIndicator.GetComponent<ChargeIndicatorController>();
            chargeIndicatorController.chargingText.gameObject.SetActive(false);
            chargeIndicatorController.chargingText.enabled = false;

            chargeIndicatorController.spriteBaseColor = Color.yellow;
            chargeIndicatorController.spriteFlashColor = Color.yellow;
            chargeIndicatorController.spriteChargedColor = Color.yellow;
            chargeIndicatorController.spriteChargingColor = Color.yellow;
        }

        private void CreatePositionLocatorLocally()
        {
            if (_teleporterLocatorObject != null) return;

            TeleporterInteraction teleporterInteraction = UnityEngine.Object.FindObjectsOfType<TeleporterInteraction>().FirstOrDefault();
            if (teleporterInteraction == null)
            {
                Factory.Logger.LogDebug($"Locator Initialize - Interaction: \"{GetType().Name}\"");
                return;
            }

            CreatePositionLocator(Guid.NewGuid(), teleporterInteraction.gameObject.transform.position);
        }

        private void DestroyLocator()
        {
            UnityEngine.Object.Destroy(_teleporterLocatorObject.gameObject);
            _teleporterLocatorObject = null;
            _teleporterLocatorId = Guid.Empty;
        }

        private void TeleporterInteraction_OnInteractionBegin(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator)
        {
            orig(self, activator);
            if (!_showTeleporterAfterStateChange.Value)
            {
                Factory.Logger.LogDebug($"Locator Hook - Hook: \"{GetType().Name}\" (action=Disable)");
                Disable();
            }
        }

        private void ToggleIndicator(bool on)
            => _teleporterLocatorObject.SetActive(on);
    }
}