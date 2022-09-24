using BepInEx.Logging;
using EasyObjectLocator.Abstract.Interfaces;
using EasyObjectLocator.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.Networking;

namespace EasyObjectLocator.Network.Messages
{
    public sealed class SyncLocatorMessage : INetMessage
    {
        public SyncLocatorMessage()
        { }

        public string ComponentId { get; private set; }
        public NetworkInstanceId InstanceId { get; private set; }
        public int InstanceType { get; private set; }
        public int SyncType { get; private set; }

        public static SyncLocatorMessage WithData(string ComponentId, NetworkInstanceId InstanceId, int instanceType, int syncType)
            => new() { ComponentId = ComponentId, InstanceId = InstanceId, InstanceType = instanceType, SyncType = syncType };

        public void Deserialize(NetworkReader reader)
        {
            ComponentId = reader.ReadString();
            InstanceId = reader.ReadNetworkId();
            InstanceType = reader.ReadInt32();
            SyncType = reader.ReadInt32();

            Factory.Logger.LogDebug($"INetMessage deserializing: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},InstanceType={InstanceType})\"");
        }

        public void OnReceived()
        {
            if (NetworkHelper.IsServer()) return;

            Factory.Logger.LogDebug($"INetMessage receiving: \" {GetType().Name} (ComponentId={ComponentId}InstanceId={InstanceId},InstanceType={InstanceType})\"");

            if (!Factory.LocatorCollection.TryGet(ComponentId, out ILocator locator))
            {
                Factory.Logger.LogError($"INetMessage received invalid component: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},InstanceType={InstanceType})\"");
                return;
            }

            locator.HandleIncomingMessage(InstanceId, InstanceType, SyncType);
        }

        public void Serialize(NetworkWriter writer)
        {
            Factory.Logger.LogDebug($"INetMessage serializing: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},InstanceType={InstanceType})\"");
            writer.Write(ComponentId);
            writer.Write(InstanceId);
            writer.Write(InstanceType);
            writer.Write(SyncType);
        }
    }
}