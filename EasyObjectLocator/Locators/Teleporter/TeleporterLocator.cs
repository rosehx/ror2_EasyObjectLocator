using BepInEx.Configuration;
using EasyObjectLocator.Abstract;
using EasyObjectLocator.Networking;
using RoR2;
using RoR2.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EasyObjectLocator.Locators.Teleporter
{
    internal sealed class TeleporterLocator : Locator
    {
        private ConfigEntry<bool> _showTeleporterAfterStateChange;
        private ConfigEntry<int> _showTeleporterAfterXSeconds;
        private ConfigEntry<bool> _showTeleporterAlways;

        public TeleporterLocator(IContext core) : base(core)
        {
        }

        public override string ComponentId => "rosehx.EasyObjectLocator.Locators.TeleporterLocator";

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

        protected override void InternalInitialize()
        {
            if (NetworkHelper.IsClient())
                return;

            CreatePositionLocatorLocally();
            if (_showTeleporterAlways.Value)
            {
                Factory.Logger.LogDebug($"Locator Initialize - ShowAlways: \"{GetType().Name}\"");
                EnableAll();
            }
            else if (!_showTeleporterAlways.Value && _showTeleporterAfterXSeconds.Value > 0)
            {
                Factory.Logger.LogDebug($"Locator Initialize - ShowDelays: \"{GetType().Name}\" (delay={_showTeleporterAfterXSeconds.Value})");
                Context.DelayedCall(EnableAll, _showTeleporterAfterXSeconds.Value);
            }
        }

        public override void RemoveHooks()
        {
            if (NetworkHelper.IsClient())
                return;
            On.RoR2.TeleporterInteraction.OnInteractionBegin -= TeleporterInteraction_OnInteractionBegin;
        }

        protected override GameObject CreateObjectInternal(NetworkInstanceId networkInstanceId)
        {
            GameObject go = Util.FindNetworkObject(networkInstanceId);

            GameObject chargingIndicatorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Teleporters/TeleporterChargingPositionIndicator.prefab").WaitForCompletion();
            GameObject teleporterLocatorObject = UnityEngine.Object.Instantiate(chargingIndicatorPrefab, go.transform.position, Quaternion.identity);
            teleporterLocatorObject.SetActive(false);

            PositionIndicator positionIndicator = teleporterLocatorObject.GetComponent<PositionIndicator>();
            positionIndicator.gameObject.SetActive(false);

            ChargeIndicatorController chargeIndicatorController = positionIndicator.GetComponent<ChargeIndicatorController>();
            chargeIndicatorController.chargingText.gameObject.SetActive(false);
            chargeIndicatorController.chargingText.enabled = false;

            chargeIndicatorController.spriteBaseColor = Color.yellow;
            chargeIndicatorController.spriteFlashColor = Color.yellow;
            chargeIndicatorController.spriteChargedColor = Color.yellow;
            chargeIndicatorController.spriteChargingColor = Color.yellow;

            return teleporterLocatorObject;
        }

        private void CreatePositionLocatorLocally()
        {
            if (LocatorObjects.Any()) return;

            TeleporterInteraction teleporterInteraction = UnityEngine.Object.FindObjectsOfType<TeleporterInteraction>().FirstOrDefault();
            if (teleporterInteraction == null)
            {
                Factory.Logger.LogDebug($"Locator Initialize - NoTeleporterInteraction: \"{GetType().Name}\"");
                return;
            }

            CreateObject(teleporterInteraction.GetComponent<NetworkIdentity>().netId);
        }

        private void TeleporterInteraction_OnInteractionBegin(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator)
        {
            orig(self, activator);
            if (!_showTeleporterAfterStateChange.Value)
                DisableObject(LocatorObjects.Keys.FirstOrDefault());
        }
    }
}