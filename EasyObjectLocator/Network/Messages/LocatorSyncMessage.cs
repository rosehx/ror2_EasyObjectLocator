using EasyObjectLocator.Locators;
using EasyObjectLocator.Networking;
using EasyObjectLocator.Utilities;
using R2API.Networking.Interfaces;
using UnityEngine.Networking;

namespace EasyObjectLocator.Network.Messages
{
    public sealed class LocatorSyncMessage : INetMessage
    {
        public LocatorSyncMessage()
        { }

        public string ComponentId { get; private set; }
        public NetworkInstanceId InstanceId { get; private set; }
        public LocatorSyncType SyncType { get; private set; }

        public static LocatorSyncMessage WithData(string ComponentId, NetworkInstanceId instanceId, LocatorSyncType syncType)
            => new() { ComponentId = ComponentId, InstanceId = instanceId, SyncType = syncType };

        public void Deserialize(NetworkReader reader)
        {
            ComponentId = reader.ReadString();
            InstanceId = reader.ReadNetworkId();

            if (!EnumUtilities.TryParse(reader.ReadInt32(), out LocatorSyncType syncTypeValue))
                return;
            SyncType = syncTypeValue;

            Factory.Logger.LogDebug($"INetMessage deserializing: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},SyncType={SyncType})\"");
        }

        public void OnReceived()
        {
            if (NetworkHelper.IsServer()) return;

            Factory.Logger.LogDebug($"INetMessage receiving: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},SyncType={SyncType})\"");

            if (!Factory.LocatorCollection.TryGet(ComponentId, out ILocator locator))
            {
                Factory.Logger.LogError($"INetMessage received invalid component: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},SyncType={SyncType})\"");
                return;
            }

            locator.Synchronize(InstanceId, SyncType);
        }

        public void Serialize(NetworkWriter writer)
        {
            Factory.Logger.LogDebug($"INetMessage serializing: \" {GetType().Name} (ComponentId={ComponentId},InstanceId={InstanceId},SyncType={SyncType})\"");
            writer.Write(ComponentId);
            writer.Write(InstanceId);
            writer.Write((int)SyncType);
        }
    }
}