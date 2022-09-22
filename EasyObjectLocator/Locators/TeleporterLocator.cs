using BepInEx.Configuration;
using EasyObjectLocator.Abstraction.Components;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EasyObjectLocator.Locators
{
    internal sealed class TeleporterLocator : ObjectLocatorComponent
    {
        private PositionIndicator _positionIndicator;
        private ChargeIndicatorController _chargeIndicatorController;

        public override string ComponentId => "rosehx.EasyObjectLocator.Locators.TeleporterLocator";

        private ConfigEntry<bool> _showTeleporterAlways;
        private ConfigEntry<int> _showTeleporterAfterXSeconds;
        private ConfigEntry<bool> _showTeleporterAfterStateChange;

        public override void ExtendConfig()
        {
            ConfigFile config = Core.GetConfig();

            _showTeleporterAlways = config.Bind<bool>(
                new ConfigDefinition("Teleporter", $"{ComponentId}.ShowAlways"), 
                false,
                new ConfigDescription("Always show the teleporter location.")
            );

            _showTeleporterAfterXSeconds = config.Bind<int>(
                new ConfigDefinition("Teleporter", $"{ComponentId}.ShowAfterXSeconds"), 
                60,
                new ConfigDescription("Show teleporter after a certain amount of seconds. 0 seconds wont set a timer. If \"ShowAlways\" is also false the teleporter is never visible.")
            );

            _showTeleporterAfterStateChange = config.Bind<bool>(
                new ConfigDefinition("Teleporter", $"{ComponentId}.ShowAfterStateChange"), 
                false,
                new ConfigDescription("Normally if you interact with the teleporter a charging Icon shows up and the locater is removed. If you enabled this the locater stays visible.")
            );

        }

        public override void ExtendHooks()
        {
            On.RoR2.TeleporterInteraction.OnEnable += TeleporterInteraction_OnEnable;
            On.RoR2.TeleporterInteraction.OnDestroy += TeleporterInteraction_OnDestroy;
            On.RoR2.TeleporterInteraction.OnInteractionBegin += TeleporterInteraction_OnInteractionBegin;
        }

        private void TeleporterInteraction_OnInteractionBegin(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator)
        {
            orig(self, activator);
            if (!_showTeleporterAfterStateChange.Value)
                HideObjects();
        }

        private void TeleporterInteraction_OnEnable(On.RoR2.TeleporterInteraction.orig_OnEnable orig, TeleporterInteraction self)
        {
            orig(self);

            GameObject chargingIndicatorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Teleporters/TeleporterChargingPositionIndicator.prefab").WaitForCompletion();
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            _positionIndicator = Object.Instantiate(chargingIndicatorPrefab, self.teleporterPositionIndicator.transform.position, Quaternion.identity).GetComponent<PositionIndicator>();
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            usedObjects.Add(_positionIndicator);

            _positionIndicator.gameObject.SetActive(false);

            _chargeIndicatorController = _positionIndicator.GetComponent<ChargeIndicatorController>();
            _chargeIndicatorController.chargingText.gameObject.SetActive(false);
            _chargeIndicatorController.chargingText.enabled = false;

            _chargeIndicatorController.spriteBaseColor = Color.yellow;
            _chargeIndicatorController.spriteFlashColor = Color.yellow;
            _chargeIndicatorController.spriteChargedColor = Color.yellow;
            _chargeIndicatorController.spriteChargingColor = Color.yellow;

            if (_showTeleporterAlways.Value)
                ShowObjects();
            else if (!_showTeleporterAlways.Value && _showTeleporterAfterXSeconds.Value > 0)
                Core.DelayedCall(ShowObjects, _showTeleporterAfterXSeconds.Value);
        }

        private void TeleporterInteraction_OnDestroy(On.RoR2.TeleporterInteraction.orig_OnDestroy orig, TeleporterInteraction self)
        {
            Core.CancelInvoke();
            DestroyObjects();
            orig(self);
        }

        private void HideObjects()
        {
            if(_positionIndicator != null)
                _positionIndicator.gameObject.SetActive(false);
            if(_chargeIndicatorController != null)
                _chargeIndicatorController.gameObject.SetActive(false);
        }

        private void ShowObjects()
        {
            if (_positionIndicator != null)
                _positionIndicator.gameObject.SetActive(true);
            if (_chargeIndicatorController != null)
                _chargeIndicatorController.gameObject.SetActive(true);
        }
    }
}
