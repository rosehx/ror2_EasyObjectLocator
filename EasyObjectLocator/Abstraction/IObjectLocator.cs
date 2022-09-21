namespace EasyObjectLocator.Abstraction
{
    public interface IObjectLocator
    {
        IObjectLocator Initialize();
        void HideObjects();
        void ShowObjects();
        void DestroyObjects();
    }
}
