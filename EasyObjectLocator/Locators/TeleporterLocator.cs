using EasyObjectLocator.Abstraction;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EasyObjectLocator.Locators
{
    internal class TeleporterLocator : ObjectLocatorBase
    {
        private PositionIndicator _positionIndicator;
        private ChargeIndicatorController _chargeIndicatorController;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public TeleporterLocator(IPluginRoot pluginRoot)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            : base(pluginRoot)
        { }

        protected override void ExtendConfig()
        {
        }

        protected override void ExtendHooks()
        {
            On.RoR2.TeleporterInteraction.OnEnable += TeleporterInteraction_OnEnable;
            On.RoR2.TeleporterInteraction.OnDestroy += TeleporterInteraction_OnDestroy;
        }


        private void TeleporterInteraction_OnEnable(On.RoR2.TeleporterInteraction.orig_OnEnable orig, TeleporterInteraction self)
        {
            orig(self);

            GameObject chargingIndicatorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Teleporters/TeleporterChargingPositionIndicator.prefab").WaitForCompletion();
            _positionIndicator = _pluginRoot.InstantiateObject(chargingIndicatorPrefab, self.teleporterPositionIndicator.transform.position, Quaternion.identity).GetComponent<PositionIndicator>();
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
            _pluginRoot.CancelInvoke();
            DestroyObjects();
            orig(self);
        }


        public override void HideObjects()
        {
            _positionIndicator.gameObject.SetActive(false);
            _chargeIndicatorController.gameObject.SetActive(false);
        }

        public override void ShowObjects()
        {
            _positionIndicator.gameObject.SetActive(true);
            _chargeIndicatorController.gameObject.SetActive(true);
        }

        public override void DestroyObjects()
        {
            _pluginRoot.DestroyObject(_positionIndicator);
        }
    }
}
