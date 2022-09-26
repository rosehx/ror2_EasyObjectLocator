using UnityEngine;
using UnityEngine.Networking;

namespace EasyObjectLocator.Networking
{
    public static class NetworkHelper
    {
        public static bool IsClient()
            => RoR2.RoR2Application.isInMultiPlayer && !NetworkServer.active;

        public static bool IsServer()
            => RoR2.RoR2Application.isInMultiPlayer && NetworkServer.active;

        public static bool IsSinglePlayer()
            => RoR2.RoR2Application.isInSinglePlayer;
    }
}