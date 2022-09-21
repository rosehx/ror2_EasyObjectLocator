using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EasyObjectLocator.Abstraction;
using EasyObjectLocator.Locators;
using UnityEngine;


namespace EasyObjectLocator
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class EasyObjectLocator : BaseUnityPlugin, IPluginRoot
    {
        public const string PluginGUID = "rosehx.EasyObjectLocator";
        public const string PluginName = "EasyObjectLocator";
        public const string PluginVersion = "1.0.0";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IObjectLocatorFactory _locatorFactory;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Awake()
        {
            _locatorFactory = new ObjectLocatorFactory(this).Initialize();
            Logger.LogInfo(nameof(Awake) + " done.");
        }

        public ManualLogSource GetLogger()
            => Logger;

        public ConfigFile GetConfig()
            => Config;

        public T InstantiateObject<T>(T original, Vector3 position, Quaternion rotation) where T : UnityEngine.Object
            => Instantiate(original, position, rotation);

        public new void DestroyObject(UnityEngine.Object o)
            => Destroy(o);

    }

}