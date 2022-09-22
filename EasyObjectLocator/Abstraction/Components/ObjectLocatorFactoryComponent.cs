using EasyObjectLocator.Abstraction.Interfaces;
using System.Collections.Generic;

namespace EasyObjectLocator.Abstraction.Components
{
    internal sealed class ObjectLocatorFactoryComponent : IObjectLocatorFactory
    {
        private readonly IPlugin Core;

        private readonly Dictionary<string, IObjectLocator> _locatorInstances;

        public ObjectLocatorFactoryComponent(IPlugin plugin)
        {
            Core = plugin;
            _locatorInstances = new Dictionary<string, IObjectLocator>();
        }

        public bool AddLocatorInstance(IObjectLocator locatorInstance)
        {
            if (_locatorInstances.ContainsKey(locatorInstance.ComponentId))
                return false;
            _locatorInstances.Add(locatorInstance.ComponentId, locatorInstance);
            return true;
        }

        public IObjectLocatorFactory Initialize()
        {
            foreach (IObjectLocator locatorInstance in _locatorInstances.Values)
            {
                locatorInstance.Initialize(Core);

                locatorInstance.ExtendConfig();
            }

            foreach (IObjectLocator locatorInstance in _locatorInstances.Values)
                locatorInstance.ExtendHooks();

            return this;
        }

        public bool TryGetLocatorInstance(string locatorComponentId, out IObjectLocator locatorInstance)
            => _locatorInstances.TryGetValue(locatorComponentId, out locatorInstance);
    }
}
