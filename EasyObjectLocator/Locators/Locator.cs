using EasyObjectLocator.Abstract;
using EasyObjectLocator.Network.Messages;
using EasyObjectLocator.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EasyObjectLocator.Locators
{
    public abstract class Locator : ILocator
    {
        public abstract string ComponentId { get; }

        public bool Initialized { get; private set; }

        protected readonly IContext Context;

        // TODO: Maybe implment Semaphore lock to access locator objects because allot of async message could lead to invalid access

        protected Dictionary<NetworkInstanceId, GameObject> LocatorObjects;

        public Locator(IContext context)
        {
            Context = context;
            LocatorObjects = new Dictionary<NetworkInstanceId, GameObject>();
        }

        public void DestroyObjects()
        {
            Factory.Logger.LogDebug($"Locator - DestroyObjects: \"{GetType()}\"");
            List<NetworkInstanceId> instanceIds = LocatorObjects.Keys.ToList();
            foreach (NetworkInstanceId instanceId in instanceIds)
                DestroyObject(instanceId);
        }

        public abstract void ExtendConfig();

        public abstract void ExtendHooks();

        public void Initialize()
        {
            InternalInitialize();
            Initialized = true;
        }

        protected abstract void InternalInitialize();

        public abstract void RemoveHooks();

        public void Synchronize(NetworkInstanceId instanceId, LocatorSyncType syncType)
        {
            if (!Initialized)
            {
                Factory.Logger.LogDebug($"Locator - DelaySync: \"{GetType()}(instanceId={instanceId},syncType={syncType})\"");
                Context.DelayedCall(() => { Synchronize(instanceId, syncType); }, 0.3f);
                return;
            }

            Factory.Logger.LogDebug($"Locator - Synchronize: \"{GetType()}(syncType={syncType})\"");
            switch (syncType)
            {
                case LocatorSyncType.Enable:
                    Enable(instanceId);
                    break;

                case LocatorSyncType.Disable:
                    Disable(instanceId);
                    break;

                case LocatorSyncType.Create:
                    PreCreateObject(instanceId);
                    break;

                case LocatorSyncType.Destroy:
                    DestroyObject(instanceId);
                    break;

                default:
                    Factory.Logger.LogError($"Locator - Synchronize: \"{GetType()}(syncType={syncType})\"");
                    break;
            }
        }

        protected void PreCreateObject(NetworkInstanceId instanceId)
        {
            Factory.Logger.LogDebug($"Locator - PreCreateObject: \"{GetType()} (isServer={NetworkHelper.IsServer()},instanceId={instanceId})\"");
            if (LocatorObjects.ContainsKey(instanceId))
            {
                Factory.Logger.LogError($"Locator - PreCreateObject: \"{GetType()} (instanceId={instanceId})\"");
                return;
            }

            GameObject gameObject = CreateObject(instanceId);
            LocatorObjects.Add(instanceId, gameObject);

            if (NetworkHelper.IsServer())
                LocatorSyncMessage.WithData(
                    ComponentId,
                    instanceId,
                    LocatorSyncType.Create
                ).Send(NetworkDestination.Clients);
        }

        protected abstract GameObject CreateObject(NetworkInstanceId instanceId);

        protected void DestroyObject(NetworkInstanceId instanceId)
        {
            Factory.Logger.LogDebug($"Locator - DestroyObject: \"{GetType()} (instanceId={instanceId})\"");
            if (!LocatorObjects.TryGetValue(instanceId, out GameObject gameObject))
            {
                Factory.Logger.LogDebug($"Locator - DestroyObject: \"{GetType()} (instanceId={instanceId})\"");
                return;
            }

            UnityEngine.Object.Destroy(gameObject);
            LocatorObjects.Remove(instanceId);

            if (NetworkHelper.IsServer())
                LocatorSyncMessage.WithData(
                    ComponentId,
                    instanceId,
                    LocatorSyncType.Destroy
                ).Send(NetworkDestination.Clients);
        }

        protected void Disable(NetworkInstanceId instanceId)
        {
            Factory.Logger.LogDebug($"Locator - Disable: \"{GetType()} (instanceId={instanceId})\"");
            if (!LocatorObjects.TryGetValue(instanceId, out GameObject gameObject))
            {
                Factory.Logger.LogDebug($"Locator - Disable: \"{GetType()}(instanceId={instanceId})\"");
                return;
            }

            gameObject.SetActive(false);

            if (NetworkHelper.IsServer())
                LocatorSyncMessage.WithData(
                    ComponentId,
                    instanceId,
                    LocatorSyncType.Disable
                ).Send(NetworkDestination.Clients);
        }

        protected void EnableAll()
        {
            Factory.Logger.LogDebug($"Locator - EnableAll: \"{GetType()}\"");
            foreach (NetworkInstanceId instanceId in LocatorObjects.Keys)
                Enable(instanceId);
        }

        protected void Enable(NetworkInstanceId instanceId)
        {
            Factory.Logger.LogDebug($"Locator - Enable: \"{GetType()} (instanceId={instanceId})\"");
            if (!LocatorObjects.TryGetValue(instanceId, out GameObject gameObject))
            {
                Factory.Logger.LogError($"Locator - Enable: \"{GetType()}(instanceId={instanceId})\"");
                Factory.Logger.LogWarning($"===test==== {string.Join(", ", LocatorObjects.Keys.Select(x => x.Value))}");
                return;
            }

            gameObject.SetActive(true);

            if (NetworkHelper.IsServer())
                LocatorSyncMessage.WithData(
                    ComponentId,
                    instanceId,
                    LocatorSyncType.Enable
                ).Send(NetworkDestination.Clients);
        }
    }
}