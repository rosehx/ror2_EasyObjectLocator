using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using UnityEngine;

namespace EasyObjectLocator.Abstraction.Interfaces
{
    public interface IPlugin
    {
        public ManualLogSource GetLogger();
        public ConfigFile GetConfig();
        public void CancelInvoke();
        public void Invoke(string methodName, float time);
        public Coroutine DelayedCall(Action callback, float delayInSeconds);
    }
}
