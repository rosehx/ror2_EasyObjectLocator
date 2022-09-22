namespace EasyObjectLocator.Abstraction.Interfaces
{
    public interface IObjectLocatorFactory
    {
        internal IObjectLocatorFactory Initialize();
        public bool TryGetLocatorInstance(string locatorComponentId, out IObjectLocator locatorInstance);
        public bool AddLocatorInstance(IObjectLocator locatorInstance);
    }
}
