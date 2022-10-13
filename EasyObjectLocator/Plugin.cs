using BepInEx;
using EasyObjectLocator.Abstract;
using EasyObjectLocator.Locators.Containers;
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

            NetworkingAPI.RegisterMessageType<LocatorSyncMessage>();

            Factory.LocatorCollection.SetContext(this);

            Factory.LocatorCollection.Instantiate<TeleporterLocator>();
            Factory.LocatorCollection.Instantiate<ChestLocator>();

            Factory.LocatorCollection.ExtendConfig();
            On.RoR2.Stage.Start += Hook_Stage_Start;
            On.RoR2.Run.AdvanceStage += Hook_Run_AdvanceStage;
            On.RoR2.Run.BeginGameOver += Hook_Run_BeginGameOver;
            On.RoR2.Run.OnApplicationQuit += Hook_Run_OnApplicationQuit;
        }

        private void Hook_Run_OnApplicationQuit(On.RoR2.Run.orig_OnApplicationQuit orig, Run self)
        {
            Factory.LocatorCollection.RemoveHooks();
            Factory.LocatorCollection.DestroyObjects();
            orig(self);
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
            Factory.LocatorCollection.RemoveHooks();
            Factory.LocatorCollection.DestroyObjects();
            orig(self, nextScene);
        }

        private void Hook_Run_BeginGameOver(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameEndingDef gameEndingDef)
        {
            Factory.LocatorCollection.RemoveHooks();
            Factory.LocatorCollection.DestroyObjects();
            orig(self, gameEndingDef);
        }

        private void Hook_Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);

            /*            List<Type> types = (from t in typeof(ChestRevealer).Assembly.GetTypes()
                                                      where typeof(IInteractable).IsAssignableFrom(t)
                                                      select t).ToList<Type>();

                        Factory.Logger.LogError($"#######=== {string.Join(", ", types.Select(x => x.FullName)  )} ==########");*/

            Factory.LocatorCollection.Initialize();
            Factory.LocatorCollection.ExtendHooks();
        }
    }
}