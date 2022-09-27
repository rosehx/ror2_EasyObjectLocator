using System;
using UnityEngine;

namespace EasyObjectLocator.Abstraction.Interfaces
{
    public interface IContext
    {
        public void CancelInvoke();

        public void Invoke(string methodName, float time);

        public Coroutine DelayedCall(Action callback, float delayInSeconds);
    }
}