using System;

namespace EasyObjectLocator.Utilities
{
    internal static class EnumUtilities
    {
        public static bool TryParse<T>(int enumValue, out T enumResult)
        {
            enumResult = default;

            Factory.Logger.LogDebug($"TryParseEnum: \"(type={typeof(T).Name},value={enumValue})\"");

            if (!Enum.IsDefined(typeof(T), enumValue))
            {
                Factory.Logger.LogError($"TryParseEnum - Error: \"(type={typeof(T).Name},value={enumValue})\"");
                return false;
            }

            enumResult = (T)(object)enumValue;

            return true;
        }
    }
}