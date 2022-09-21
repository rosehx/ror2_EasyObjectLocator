using EasyObjectLocator.Abstraction;

namespace EasyObjectLocator.Locators
{
    internal sealed class ObjectLocatorFactory : IObjectLocatorFactory
    {
        private readonly IObjectLocator _teleporterLocator;

        private readonly IPluginRoot _pluginRoot;

        public ObjectLocatorFactory(IPluginRoot pluginRoot)
        {
            _pluginRoot = pluginRoot;
            _teleporterLocator = new TeleporterLocator(_pluginRoot);
        }

        public IObjectLocatorFactory Initialize()
        {
            _teleporterLocator.Initialize();

            return this;
        }
    }
}
