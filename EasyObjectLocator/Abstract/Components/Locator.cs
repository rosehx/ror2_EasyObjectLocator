using EasyObjectLocator.Abstract.Interfaces;
using EasyObjectLocator.Abstraction.Interfaces;
using System;
using UnityEngine.Networking;

namespace EasyObjectLocator.Abstraction.Components
{
    public abstract class Locator : ILocator
    {
        public Locator(IContext context)
        {
            Context = context;
        }

        public abstract string ComponentId { get; }

        protected IContext Context { get; private set; }

        public abstract void DestroyObjects();

        protected abstract void Enable();

        protected abstract void Disable();

        public abstract void ExtendConfig();

        public abstract void ExtendHooks();

        public abstract void HandleIncomingMessage(NetworkInstanceId instanceId, int instanceType, int syncType);

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