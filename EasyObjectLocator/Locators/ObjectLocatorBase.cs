using EasyObjectLocator.Abstraction;

namespace EasyObjectLocator.Locators
{
    internal abstract class ObjectLocatorBase : IObjectLocator
    {
        protected readonly IPluginRoot _pluginRoot;

        protected bool _visible = false;

        public ObjectLocatorBase(IPluginRoot pluginRoot)
        {
            _pluginRoot = pluginRoot;
        }

        public IObjectLocator Initialize()
        {
            ExtendConfig();
            ExtendHooks();

            return this;
        }

        protected virtual void ExtendConfig() { }
        protected virtual void ExtendHooks() { }

        public abstract void HideObjects();
        public abstract void ShowObjects();
        public abstract void DestroyObjects();
    }
}
