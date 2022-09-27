using EasyObjectLocator.Abstract.Interfaces;
using EasyObjectLocator.Abstraction.Interfaces;
using System;
using System.Collections.Generic;

namespace EasyObjectLocator.Abstraction.Components
{
    internal sealed class LocatorCollection : ILocatorCollection
    {
        private IContext Context;

        private readonly Dictionary<string, ILocator> _locatorInstances;

        public LocatorCollection()
        {
            _locatorInstances = new Dictionary<string, ILocator>();
        }

        public bool Instantiate<T>() where T : class, ILocator
        {
            if (Context == null) throw new AccessViolationException("Context not set");

            Factory.Logger.LogDebug($"LocationCollection Instantiate - Pre: \"{typeof(T)}\"");

            ILocator locatorInstance = (ILocator)Activator.CreateInstance(typeof(T), Context);

            Factory.Logger.LogDebug($"LocationCollection Instantiate - Init: \"{typeof(T)} (ComponentId={locatorInstance.ComponentId})\"");

            if (_locatorInstances.ContainsKey(locatorInstance.ComponentId))
                return false;

            Factory.Logger.LogDebug($"LocationCollection Instantiate - Add: \"{typeof(T)} (ComponentId={locatorInstance.ComponentId})\"");

            _locatorInstances.Add(locatorInstance.ComponentId, locatorInstance);

            return true;
        }

        public bool TryGet(string locatorComponentId, out ILocator locatorInstance)
        {
            if (Context == null) throw new AccessViolationException("Context not set");

            bool found = _locatorInstances.TryGetValue(locatorComponentId, out locatorInstance);

            Factory.Logger.LogDebug($"LocationCollection TryGet: \"{locatorComponentId} (found={found})\"");

            return found;
        }

        public void DestroyObjects()
        {
            Factory.Logger.LogDebug($"LocationCollection DestroyObjects - Pre");
            if (Context == null) throw new AccessViolationException("Context not set");
            foreach (ILocator locatorInstance in _locatorInstances.Values)
            {
                Factory.Logger.LogDebug($"LocationCollection DestroyObjects - Itr: \"{locatorInstance.ComponentId}\"");
                locatorInstance.DestroyObjects();
            }
        }

        public void InitializeObjects()
        {
            Factory.Logger.LogDebug($"LocationCollection InitializeObjects - Pre");
            if (Context == null) throw new AccessViolationException("Context not set");
            foreach (ILocator locatorInstance in _locatorInstances.Values)
            {
                Factory.Logger.LogDebug($"LocationCollection InitializeObjects - Itr: \"{locatorInstance.ComponentId}\"");
                locatorInstance.Initialize();
            }
        }

        public void ExtendConfig()
        {
            Factory.Logger.LogDebug($"LocationCollection ExtendConfig - Pre");
            if (Context == null) throw new AccessViolationException("Context not set");
            foreach (ILocator locatorInstance in _locatorInstances.Values)
            {
                Factory.Logger.LogDebug($"LocationCollection ExtendConfig - Itr: \"{locatorInstance.ComponentId}\"");
                locatorInstance.ExtendConfig();
            }
        }

        public void ExtendHooks()
        {
            Factory.Logger.LogDebug($"LocationCollection ExtendHooks - Pre");
            if (Context == null) throw new AccessViolationException("Context not set");
            foreach (ILocator locatorInstance in _locatorInstances.Values)
            {
                Factory.Logger.LogDebug($"LocationCollection ExtendHooks - Itr: \"{locatorInstance.ComponentId}\"");
                locatorInstance.ExtendHooks();
            }
        }

        public void RemoveHooks()
        {
            Factory.Logger.LogDebug($"LocationCollection RemoveHooks - Pre");
            if (Context == null) throw new AccessViolationException("Context not set");
            foreach (ILocator locatorInstance in _locatorInstances.Values)
            {
                Factory.Logger.LogDebug($"LocationCollection RemoveHooks - Itr: \"{locatorInstance.ComponentId}\"");
                locatorInstance.RemoveHooks();
            }
        }

        public void SetContext(IContext context)
        {
            if (Context != null) throw new AccessViolationException("Context already set.");
            Factory.Logger.LogDebug($"LocationCollection SetContext");
            Context = context;
        }
    }
}