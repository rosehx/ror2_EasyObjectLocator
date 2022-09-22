using EasyObjectLocator.Abstraction.Interfaces;

namespace EasyObjectLocator
{
    public static class Factory
    {
        public static IObjectLocatorFactory Instance { get; internal set; }
    }
}
