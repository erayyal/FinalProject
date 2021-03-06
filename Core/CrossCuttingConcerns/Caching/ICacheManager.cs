namespace Core.CrossCuttingConcerns.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        object Get(string key);
        void Add(string key, object value, int duration);
        bool IsAdd(string key); //cache'te var mı
        void Remove(string key); //cache'ten uçurma
        void RemoveByPattern(string pattern);
    }
}