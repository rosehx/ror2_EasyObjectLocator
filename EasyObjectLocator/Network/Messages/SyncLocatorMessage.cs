using BepInEx.Logging;
using EasyObjectLocator.Locators;
using EasyObjectLocator.Networking;
using R2API.Networking.Interfaces;
using RoR2.Networking;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EasyObjectLocator.Network.Messages
{
    public sealed class SyncLocatorMessage : INetMessage
    {
        public SyncLocatorMessage()
        { }

        public string ComponentId { get; private set; }
        public Guid InstanceId { get; private set; }
        public Vector3 Position { get; private set; }
        public int InstanceType { get; private set; }
        public int SyncType { get; private set; }

        public static SyncLocatorMessage WithData(string ComponentId, Guid instanceId, Vector3 position, int instanceType, int syncType)
            => new() { ComponentId = ComponentId, InstanceId = instanceId, Position = position, InstanceType = instanceType, SyncType = syncType };

        public void Deserialize(NetworkReader reader)
        {
            ComponentId = reader.ReadString();
            InstanceId = reader.ReadGuid();
            Position = reader.ReadVector3();
            InstanceType = reader.ReadInt32();
            SyncType = reader.ReadInt32();

            Factory.Logger.LogDebug($"INetMessage deserializing: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},Position{Position},InstanceType={InstanceType},SyncType={SyncType})\"");
        }

        public void OnReceived()
        {
            if (NetworkHelper.IsServer()) return;

            Factory.Logger.LogDebug($"INetMessage receiving: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},Position{Position},InstanceType={InstanceType},SyncType={SyncType})\"");

            if (!Factory.LocatorCollection.TryGet(ComponentId, out ILocator locator))
            {
                Factory.Logger.LogError($"INetMessage received invalid component: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},Position{Position},InstanceType={InstanceType},SyncType={SyncType})\"");
                return;
            }

            locator.HandleIncomingMessage(InstanceId, Position, InstanceType, SyncType);
        }

        public void Serialize(NetworkWriter writer)
        {
            Factory.Logger.LogDebug($"INetMessage serializing: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},Position{Position},InstanceType={InstanceType},SyncType={SyncType})\"");
            writer.Write(ComponentId);
            writer.WriteGuid(InstanceId);
            writer.Write(Position);
            writer.Write(InstanceType);
            writer.Write(SyncType);
        }
    }
}