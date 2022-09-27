using BepInEx;
using EasyObjectLocator.Abstraction.Interfaces;
using EasyObjectLocator.Locators.Teleporter;
using EasyObjectLocator.Network.Messages;
using R2API;
using R2API.Networking;
using R2API.Utils;
using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace EasyObjectLocator
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(PrefabAPI))]
    [BepInPlugin(Constants.PluginGUID, Constants.PluginName, Constants.PluginVersion)]
    public class Plugin : BaseUnityPlugin, IContext
    {
        public void Awake()
        {

#if DEBUG
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
#endif

            NetworkingAPI.RegisterMessageType<SyncLocatorMessage>();

            Factory.LocatorCollection.SetContext(this);

            Factory.LocatorCollection.Instantiate<TeleporterLocator>();

            Factory.LocatorCollection.ExtendConfig();

            On.RoR2.Stage.Start += Hook_Stage_Start;
            On.RoR2.Run.AdvanceStage += Hook_Run_AdvanceStage;
            On.RoR2.Run.BeginGameOver += Hook_Run_BeginGameOver;
        }

        public Coroutine DelayedCall(Action callback, float delayInSeconds)
            => StartCoroutine(DelayedCallInternal(callback, delayInSeconds));

        private IEnumerator DelayedCallInternal(Action callback, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            callback();
        }

        private void Hook_Run_AdvanceStage(On.RoR2.Run.orig_AdvanceStage orig, Run self, SceneDef nextScene)
        {
            orig(self, nextScene);
            Factory.LocatorCollection.RemoveHooks();
            Factory.LocatorCollection.DestroyObjects();
        }

        private void Hook_Run_BeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameEndingDef gameEndingDef)
        {
            orig(self, gameEndingDef);
            Factory.LocatorCollection.RemoveHooks();
            Factory.LocatorCollection.DestroyObjects();
        }

        private void Hook_Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            Factory.LocatorCollection.InitializeObjects();
            Factory.LocatorCollection.ExtendHooks();
        }
    }
}