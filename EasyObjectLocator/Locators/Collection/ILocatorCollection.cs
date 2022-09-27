using EasyObjectLocator.Abstract;
using EasyObjectLocator.Locators;

namespace EasyObjectLocator.Collection
{
    public interface ILocatorCollection
    {
        internal abstract void SetContext(IContext context);

        internal abstract void ExtendConfig();

        internal abstract void InitializeObjects();

        internal abstract void ExtendHooks();

        internal abstract void RemoveHooks();

        internal abstract void DestroyObjects();

        public abstract bool TryGet(string locatorComponentId, out ILocator locatorInstance);

        public bool Instantiate<T>() where T : class, ILocator;
    }
}