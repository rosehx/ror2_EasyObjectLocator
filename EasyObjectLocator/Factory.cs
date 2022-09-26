using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EasyObjectLocator.Abstract.Interfaces;
using EasyObjectLocator.Abstraction.Components;

namespace EasyObjectLocator
{
    public static class Factory
    {
        private static ILocatorCollection _locatorCollection;

        public static ILocatorCollection LocatorCollection
        {
            get => _locatorCollection ??= new LocatorCollection();
        }

        private static ManualLogSource _manuallogSource;

        public static ManualLogSource Logger
        {
            get => _manuallogSource ??= BepInEx.Logging.Logger.CreateLogSource(Constants.PluginName);
        }

        private static ConfigFile _configFile;

        public static ConfigFile Config
        {
            get => _configFile ??= new ConfigFile(Utility.CombinePaths(Paths.ConfigPath, Constants.PluginGUID + ".cfg"), false);
        }
    }
}