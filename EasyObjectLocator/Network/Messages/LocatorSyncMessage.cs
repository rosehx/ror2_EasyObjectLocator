using EasyObjectLocator.Locators;
using EasyObjectLocator.Networking;
using EasyObjectLocator.Utilities;
using R2API.Networking.Interfaces;
using RoR2.Networking;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EasyObjectLocator.Network.Messages
{
    public sealed class LocatorSyncMessage : INetMessage
    {
        public LocatorSyncMessage()
        { }

        public string ComponentId { get; private set; }
        public Guid InstanceId { get; private set; }
        public Vector3 Position { get; private set; }
        public LocatorSyncType SyncType { get; private set; }

        public static LocatorSyncMessage WithData(string ComponentId, Guid instanceId, Vector3 position, LocatorSyncType syncType)
            => new() { ComponentId = ComponentId, InstanceId = instanceId, Position = position, SyncType = syncType };

        public void Deserialize(NetworkReader reader)
        {
            ComponentId = reader.ReadString();
            InstanceId = reader.ReadGuid();
            Position = reader.ReadVector3();

            if (!EnumUtilities.TryParse(reader.ReadInt32(), out LocatorSyncType syncTypeValue))
                return;
            SyncType = syncTypeValue;

            Factory.Logger.LogDebug($"INetMessage deserializing: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},Position{Position},SyncType={SyncType})\"");
        }

        public void OnReceived()
        {
            if (NetworkHelper.IsServer()) return;

            Factory.Logger.LogDebug($"INetMessage receiving: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},Position{Position},SyncType={SyncType})\"");

            if (!Factory.LocatorCollection.TryGet(ComponentId, out ILocator locator))
            {
                Factory.Logger.LogError($"INetMessage received invalid component: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},Position{Position},SyncType={SyncType})\"");
                return;
            }

            locator.Synchronize(InstanceId, Position, SyncType);
        }

        public void Serialize(NetworkWriter writer)
        {
            Factory.Logger.LogDebug($"INetMessage serializing: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},Position{Position},SyncType={SyncType})\"");
            writer.Write(ComponentId);
            writer.WriteGuid(InstanceId);
            writer.Write(Position);
            writer.Write((int)SyncType);
        }
    }
}