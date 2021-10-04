namespace GameCore.Database
{
    public interface IDatabase
    {
        T GetItem<T>(string path) where T : class;
    }
}
