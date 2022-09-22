using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EasyObjectLocator.Abstraction.Components;
using EasyObjectLocator.Abstraction.Interfaces;
using EasyObjectLocator.Locators;
using UnityEngine;

namespace EasyObjectLocator
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]

    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin, IPluginRoot
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

        public ManualLogSource GetLogger()
            => Logger;

        public ConfigFile GetConfig()
            => Config;

        public T InstantiateObject<T>(T original, Vector3 position, Quaternion rotation) where T : Object
            => Instantiate(original, position, rotation);

        public new void DestroyObject(Object o)
            => Destroy(o);

    }

}