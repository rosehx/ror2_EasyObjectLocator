using EasyObjectLocator.Abstraction.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace EasyObjectLocator.Abstraction.Components
{
    public abstract class ObjectLocatorComponent : IObjectLocator
    {
        protected IPluginRoot PluginRoot { get; private set; }

        public abstract string ComponentId { get; }

        public ObjectLocatorComponent()
        {
            usedObjects = new List<Object>();
        }

        public IObjectLocator Initialize(IPluginRoot pluginRoot)
        {
            PluginRoot = pluginRoot;

            return this;
        }

        public abstract void ExtendConfig();
        public abstract void ExtendHooks();

        protected List<Object> usedObjects;

        public void DestroyObjects()
        {
            foreach(Object o in usedObjects)
                PluginRoot.DestroyObject(o);
            usedObjects = new List<Object>();
        }
    }
}
