using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EasyObjectLocator.Abstraction.Components;
using EasyObjectLocator.Abstraction.Interfaces;
using EasyObjectLocator.Locators;
using System.Collections;
using UnityEngine;
using System;
using Mono.Cecil;

namespace EasyObjectLocator
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin, IPlugin
    {
        public const string PluginGUID = "com.rosehx.easyobjectlocator";
        public const string PluginName = "EasyObjectLocator";
        public const string PluginVersion = "1.0.0";

        public void Awake()
        {
            Factory.Instance = new ObjectLocatorFactoryComponent(this);

            Factory.Instance.AddLocatorInstance(new TeleporterLocator());

            Factory.Instance.Initialize();
        }

        private IEnumerator DelayedCallInternal(Action callback, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            callback();
        }

        public Coroutine DelayedCall(Action callback, float delayInSeconds)
            => StartCoroutine(DelayedCallInternal(callback, delayInSeconds));

        public ManualLogSource GetLogger()
            => Logger;

        public ConfigFile GetConfig()
            => Config;

    }

}