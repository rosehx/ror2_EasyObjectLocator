namespace EasyObjectLocator.Abstraction.Interfaces
{
    public interface IObjectLocator
    {
        public string ComponentId { get; }
        public abstract void ExtendConfig();
        public abstract void ExtendHooks();
        internal IObjectLocator Initialize(IPluginRoot pluginRoot);
        public void DestroyObjects();
    }
}
