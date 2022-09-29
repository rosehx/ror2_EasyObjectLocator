using EasyObjectLocator.Abstract;
using EasyObjectLocator.Network.Messages;
using EasyObjectLocator.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyObjectLocator.Locators
{
    public abstract class Locator : ILocator
    {
        public abstract string ComponentId { get; }

        protected readonly IContext Context;

        // TODO: Maybe implment Semaphore lock to access locator objects because allot of async message could lead to invalid access

        protected Dictionary<Guid, GameObject> LocatorObjects;

        public Locator(IContext context)
        {
            Context = context;
            LocatorObjects = new Dictionary<Guid, GameObject>();
        }

        public void DestroyObjects()
        {
            Factory.Logger.LogDebug($"Locator - DestroyObjects: \"{GetType()}\"");
            List<Guid> instanceIds = LocatorObjects.Keys.ToList();
            foreach (Guid instanceId in instanceIds)
                DestroyObject(instanceId);
        }

        public abstract void ExtendConfig();

        public abstract void ExtendHooks();

        public abstract void Initialize();

        public abstract void RemoveHooks();

        public void Synchronize(Guid instanceId, Vector3 position, LocatorSyncType syncType)
        {
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
                    CreateObject(instanceId, position);
                    break;

                case LocatorSyncType.Destroy:
                    DestroyObject(instanceId);
                    break;

                default:
                    Factory.Logger.LogError($"Locator - Synchronize: \"{GetType()}(syncType={syncType})\"");
                    break;
            }
        }

        protected void CreateObject(Guid instanceId, Vector3 position)
        {
            Factory.Logger.LogDebug($"Locator - CreateObject: \"{GetType()} (isServer={NetworkHelper.IsServer()},instanceId={instanceId},position={position})\"");
            if (LocatorObjects.ContainsKey(instanceId))
            {
                Factory.Logger.LogError($"Locator - CreateObject: \"{GetType()} (instanced={instanceId},position={position})\"");
                return;
            }
            GameObject gameObject = CreateObject(position);
            LocatorObjects.Add(instanceId, gameObject);

            if (NetworkHelper.IsServer())
                LocatorSyncMessage.WithData(
                    ComponentId,
                    instanceId,
                    position,
                    LocatorSyncType.Create
                ).Send(NetworkDestination.Clients);
        }

        protected abstract GameObject CreateObject(Vector3 position);

        protected void DestroyObject(Guid instanceId)
        {
            Factory.Logger.LogDebug($"Locator - DestroyObject: \"{GetType()} (instanced={instanceId})\"");
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
                    new Vector3(),
                    LocatorSyncType.Destroy
                ).Send(NetworkDestination.Clients);
        }

        protected void Disable(Guid instanceId)
        {
            Factory.Logger.LogDebug($"Locator - Disable: \"{GetType()} (instanced={instanceId})\"");
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
                    new Vector3(),
                    LocatorSyncType.Disable
                ).Send(NetworkDestination.Clients);
        }

        protected void Enable(Guid instanceId)
        {
            Factory.Logger.LogDebug($"Locator - Enable: \"{GetType()} (instanced={instanceId})\"");
            if (!LocatorObjects.TryGetValue(instanceId, out GameObject gameObject))
            {
                Factory.Logger.LogError($"Locator - Enable: \"{GetType()}(instanceId={instanceId})\"");
                return;
            }

            gameObject.SetActive(true);

            if (NetworkHelper.IsServer())
                LocatorSyncMessage.WithData(
                    ComponentId,
                    instanceId,
                    new Vector3(),
                    LocatorSyncType.Enable
                ).Send(NetworkDestination.Clients);
        }
    }
}