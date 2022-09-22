using EasyObjectLocator.Abstraction.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace EasyObjectLocator.Abstraction.Components
{
    public abstract class ObjectLocatorComponent : IObjectLocator
    {
        protected IPlugin Core { get; private set; }

        public abstract string ComponentId { get; }

        public ObjectLocatorComponent()
        {
            usedObjects = new List<Object>();
        }

        public IObjectLocator Initialize(IPlugin plugin)
        {
            Core = plugin;

            return this;
        }

        public abstract void ExtendConfig();
        public abstract void ExtendHooks();

        protected List<Object> usedObjects;

        public void DestroyObjects()
        {
            foreach(Object o in usedObjects)
                Object.Destroy(o);
            usedObjects = new List<Object>();
        }
    }
}
