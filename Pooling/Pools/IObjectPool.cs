namespace MakePdf.Pooling.Pools
{
    public interface IObjectPool<T>
    {
        T Fetch();
        void Store(T obj);
    }
}
