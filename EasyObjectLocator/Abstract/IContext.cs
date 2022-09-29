using System;
using UnityEngine;

namespace EasyObjectLocator.Abstract
{
    public interface IContext
    {
        public void CancelInvoke();

        public void Invoke(string methodName, float time);

        public Coroutine DelayedCall(Action callback, float delayInSeconds);
    }
}