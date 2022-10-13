using EasyObjectLocator.Network.Messages;
using UnityEngine.Networking;

namespace EasyObjectLocator.Locators
{
    public interface ILocator
    {
        public abstract string ComponentId { get; }

        public abstract void ExtendConfig();

        public abstract void ExtendHooks();

        public void DestroyObjects();

        public void Initialize();

        public abstract void RemoveHooks();

        public void Synchronize(NetworkInstanceId instanceId, LocatorSyncType syncType);
    }
}