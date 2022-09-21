using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace EasyObjectLocator.Abstraction
{
    public interface IPluginRoot
    {
        ManualLogSource GetLogger();
        ConfigFile GetConfig();
        T InstantiateObject<T>(T original, Vector3 position, Quaternion rotation) where T : UnityEngine.Object;
        void DestroyObject(UnityEngine.Object o);
        void CancelInvoke();
    }
}
