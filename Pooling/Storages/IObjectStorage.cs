namespace MakePdf.Pooling.Storages
{
    public interface IObjectStorage<T>
    {
        T Get();
        void Return(T obj);
    }
}
