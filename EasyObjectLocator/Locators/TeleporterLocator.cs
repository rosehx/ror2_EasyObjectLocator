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

        public override string ComponentId => "rosehx.EasyObjectLocator.TeleporterLocator";

        public override void ExtendConfig()
        {
        }

        public override void ExtendHooks()
        {
            On.RoR2.TeleporterInteraction.OnEnable += TeleporterInteraction_OnEnable;
            On.RoR2.TeleporterInteraction.OnDestroy += TeleporterInteraction_OnDestroy;
        }

        private void TeleporterInteraction_OnEnable(On.RoR2.TeleporterInteraction.orig_OnEnable orig, TeleporterInteraction self)
        {
            orig(self);

            GameObject chargingIndicatorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Teleporters/TeleporterChargingPositionIndicator.prefab").WaitForCompletion();
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            _positionIndicator = PluginRoot.InstantiateObject(chargingIndicatorPrefab, self.teleporterPositionIndicator.transform.position, Quaternion.identity).GetComponent<PositionIndicator>();
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

            ShowObjects();
        }

        private void TeleporterInteraction_OnDestroy(On.RoR2.TeleporterInteraction.orig_OnDestroy orig, TeleporterInteraction self)
        {
            PluginRoot.CancelInvoke();
            DestroyObjects();
            orig(self);
        }


        private void HideObjects()
        {
            _positionIndicator.gameObject.SetActive(false);
            _chargeIndicatorController.gameObject.SetActive(false);
        }

        private void ShowObjects()
        {
            _positionIndicator.gameObject.SetActive(true);
            _chargeIndicatorController.gameObject.SetActive(true);
        }
    }
}
