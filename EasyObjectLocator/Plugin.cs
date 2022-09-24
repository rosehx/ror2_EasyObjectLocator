using BepInEx;
using EasyObjectLocator.Abstraction.Interfaces;
using EasyObjectLocator.Locators.Teleporter;
using EasyObjectLocator.Network.Messages;
using R2API.Networking;
using R2API.Utils;
using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace EasyObjectLocator
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(nameof(NetworkingAPI))]
    [BepInPlugin(Constants.PluginGUID, Constants.PluginName, Constants.PluginVersion)]
    public class Plugin : BaseUnityPlugin, IContext
    {
        public void Awake()
        {
            NetworkingAPI.RegisterMessageType<SyncLocatorMessage>();

            Factory.LocatorCollection.SetContext(this);

            Factory.LocatorCollection.Instantiate<TeleporterLocator>();

            Factory.LocatorCollection.ExtendConfig();

            On.RoR2.Stage.Start += Hook_Stage_Start;
            On.RoR2.Run.EndStage += Hook_Run_EndStage;
        }

        private void Hook_Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            Factory.LocatorCollection.InitializeObjects();
            Factory.LocatorCollection.ExtendHooks();
        }

        private void Hook_Run_EndStage(On.RoR2.Run.orig_EndStage orig, RoR2.Run self)
        {
            orig(self);
            Factory.LocatorCollection.RemoveHooks();
            Factory.LocatorCollection.DestroyObjects();
        }

        private IEnumerator DelayedCallInternal(Action callback, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            callback();
        }

        public Coroutine DelayedCall(Action callback, float delayInSeconds)
            => StartCoroutine(DelayedCallInternal(callback, delayInSeconds));
    }
}