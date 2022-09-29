using EasyObjectLocator.Network.Messages;
using System;
using UnityEngine;

namespace EasyObjectLocator.Locators
{
    public interface ILocator
    {
        public abstract string ComponentId { get; }

        public abstract void ExtendConfig();

        public abstract void ExtendHooks();

        public void DestroyObjects();

        public abstract void Initialize();

        public abstract void RemoveHooks();

        public void Synchronize(Guid instanceId, Vector3 position, LocatorSyncType syncType);
    }
}