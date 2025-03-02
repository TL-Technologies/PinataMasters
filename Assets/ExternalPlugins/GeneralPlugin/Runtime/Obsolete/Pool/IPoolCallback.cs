namespace Modules.General.Obsolete
{
    public interface IPoolCallback 
    {
        void OnCreateFromPool();
        
        void OnReturnToPool();

        void OnPop();
        
        void OnPush();
    }
}
