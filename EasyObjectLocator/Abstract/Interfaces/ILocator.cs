using System;
using UnityEngine;

namespace EasyObjectLocator.Abstract.Interfaces
{
    public interface ILocator
    {
        public abstract string ComponentId { get; }

        public abstract void ExtendConfig();

        public abstract void ExtendHooks();

        public abstract void DestroyObjects();

        public abstract void Initialize();

        public abstract void RemoveHooks();

        public abstract void HandleIncomingMessage(Guid instanceId, Vector3 position, int instanceType, int syncType);
    }
}