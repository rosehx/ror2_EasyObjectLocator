using BepInEx.Configuration;
using EasyObjectLocator.Abstract;
using EasyObjectLocator.Networking;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EasyObjectLocator.Locators.Containers
{
    internal sealed class ChestLocator : Locator
    {
        private ConfigEntry<string> _showChestsAfterTelporterStateChange;
        private ConfigEntry<int> _showChestsAfterXSeconds;
        private ConfigEntry<bool> _showChestsAlways;

        protected override bool SyncManually => false;

        public ChestLocator(IContext core) : base(core)
        {
        }

        public override string ComponentId => "rosehx.EasyObjectLocator.Locators.ChestLocator";

        public override void ExtendConfig()
        {
            ConfigFile config = Factory.Config;

            _showChestsAlways = config.Bind(
                new ConfigDefinition("Chests", $"{ComponentId}.ShowAlways"),
                false,
                new ConfigDescription("Always show chests")
            );

            _showChestsAfterXSeconds = config.Bind(
                new ConfigDefinition("Chests", $"{ComponentId}.ShowAfterXSeconds"),
                0,
                new ConfigDescription("Show chests after X amount of seconds < 1 means off.")
            );

            _showChestsAfterTelporterStateChange = config.Bind(
                new ConfigDefinition("Chests", $"{ComponentId}.ShowAfterStateChange"),
                "TeleporterCharged",
                new ConfigDescription("This ensures all chests become visible once teleporter changes state.", new AcceptableValueList<string>("Off", "TeleporterCharging", "TeleporterCharged"))
            );
        }

        public override void ExtendHooks()
        {
            if (NetworkHelper.IsClient())
                return;
            On.RoR2.TeleporterInteraction.OnInteractionBegin += Hook_Teleporter_Start_Charging;
            TeleporterInteraction.onTeleporterChargedGlobal += Hook_Teleporter_Finished_Charging;

            // TODO: Add hook for teleporter finished charging

            // TODO: Add hook for on chest open destroy
        }

        private void Hook_Teleporter_Finished_Charging(TeleporterInteraction obj)
        {
            if (_showChestsAfterTelporterStateChange.Value == "TeleporterCharged")
                EnableAll();
        }

        private void Hook_Teleporter_Start_Charging(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, RoR2.TeleporterInteraction self, RoR2.Interactor activator)
        {
            orig(self, activator);
            if (_showChestsAfterTelporterStateChange.Value == "TeleporterCharging")
                EnableAll();
        }

        protected override void InternalInitialize()
        {
            if (NetworkHelper.IsClient())
                return;

            CreateAllChestsLocally();
            if (_showChestsAlways.Value)
            {
                Factory.Logger.LogDebug($"Locator Initialize - ShowAlways: \"{GetType().Name}\"");
                EnableAll();
            }
            else if (!_showChestsAlways.Value && _showChestsAfterXSeconds.Value > 0)
            {
                Factory.Logger.LogDebug($"Locator Initialize - ShowDelays: \"{GetType().Name}\" (delay={_showChestsAfterXSeconds.Value})");
                Context.DelayedCall(EnableAll, _showChestsAfterXSeconds.Value);
            }
        }

        private void CreateAllChestsLocally()
        {
            foreach (ChestBehavior cb in Object.FindObjectsOfType<ChestBehavior>())
            {
                NetworkIdentity netId = cb.gameObject.GetComponent<NetworkIdentity>();
                if (netId != null)
                    CreateObject(netId.netId);
            }
        }

        public override void RemoveHooks()
        {
            if (NetworkHelper.IsClient())
                return;
            On.RoR2.TeleporterInteraction.OnInteractionBegin -= Hook_Teleporter_Start_Charging;
            TeleporterInteraction.onTeleporterChargedGlobal -= Hook_Teleporter_Finished_Charging;
        }

        protected override void EnableObjectInternal(GameObject gameObject)
        {
/*            ChestRevealer.RevealedObject ro = gameObject.gameObject.AddComponent<ChestRevealer.RevealedObject>();

            ro.SetFieldValue("lifetime", float.MaxValue);

            ro.gameObject.SetActive(true);*/
/*            ChestRevealer.RevealedObject ro = gameObject.gameObject.GetComponent<ChestRevealer.RevealedObject>();
            ro.enabled = true;*/
        }

        protected override GameObject CreateObjectInternal(NetworkInstanceId networkInstanceId)
        {
            GameObject go = Util.FindNetworkObject(networkInstanceId);

            ChestRevealer.RevealedObject ro = go.gameObject.AddComponent<ChestRevealer.RevealedObject>();

            ro.SetFieldValue("lifetime", float.MaxValue);

            ro.enabled = true;
            ro.gameObject.SetActive(true);

            return go;
        }
    }
}