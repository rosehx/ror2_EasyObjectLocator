using UnityEngine.Networking;

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

        public abstract void HandleIncomingMessage(NetworkInstanceId instanceId, int instanceType, int syncType);
    }
}