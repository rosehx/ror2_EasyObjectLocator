using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace EasyObjectLocator.Abstraction.Interfaces
{
    public interface IPluginRoot
    {
        public ManualLogSource GetLogger();
        public ConfigFile GetConfig();
        public T InstantiateObject<T>(T original, Vector3 position, Quaternion rotation) where T : Object;
        public void DestroyObject(Object o);
        public void CancelInvoke();
    }
}
