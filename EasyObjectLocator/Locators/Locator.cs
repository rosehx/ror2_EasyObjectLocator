using EasyObjectLocator.Abstract;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyObjectLocator.Locators
{
    public abstract class Locator : ILocator
    {
        protected Dictionary<Guid, GameObject> LocatorObjects;

        public Locator(IContext context)
        {
            Context = context;
            LocatorObjects = new Dictionary<Guid, GameObject>();
        }

        public abstract string ComponentId { get; }

        protected IContext Context { get; private set; }

        public abstract void DestroyObjects();

        protected abstract void Enable();

        protected abstract void Disable();

        public abstract void ExtendConfig();

        public abstract void ExtendHooks();

        public abstract void HandleIncomingMessage(Guid instanceId, Vector3 position, int instanceType, int syncType);

        public abstract void Initialize();

        public abstract void RemoveHooks();

        protected bool TryParseEnum<T>(int enumValue, out T enumResult)
        {
            enumResult = default;

            Factory.Logger.LogDebug($"Locator TryParseEnum: \"{GetType().Name} (type={typeof(T).Name},value={enumValue})\"");

            if (!Enum.IsDefined(typeof(T), enumValue))
            {
                Factory.Logger.LogError($"Locator TryParseEnum - Error: \"{GetType().Name} (type={typeof(T).Name},value={enumValue})\"");
                return false;
            }

            enumResult = (T)(object)enumValue;

            return true;
        }
    }
}